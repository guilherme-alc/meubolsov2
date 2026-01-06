using System.Text.Json;
using MeuBolso.Application.Auth.Abstractions;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace MeuBolso.API.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _env;
    private bool _isDevelopment => _env.IsDevelopment();

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            // Cliente cancelou
            if (!context.Response.HasStarted)
                context.Response.StatusCode = 499; // Client Closed Request
        }
        catch (DbUpdateConcurrencyException ex)
        {
            var message = "Conflito de concorrência. Tente novamente.";
            var errorResponse = new
            {
                Message = _isDevelopment ? $"{message}: {ex.Message}" : message,
                Details = _isDevelopment ? ex.StackTrace : ""
            };
            await WriteErrorAsync(context, 
                StatusCodes.Status409Conflict, 
                errorResponse);
        }
        catch (DbUpdateException ex) when (TryMapDbUpdate(ex, out var status, out var message))
        {
            _logger.LogError(ex, "Erro ao persistir dados");
            var errorResponse = new
            {
                Message = message,
                Details = _isDevelopment ? ex.Message : ""
            };
            await WriteErrorAsync(context, status, errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado");
            var errorResponse = new
            {
                Message = _isDevelopment ? ex.Message : "Erro interno no servidor",
                Details = _isDevelopment ? ex.StackTrace : ""
            };
            
            await WriteErrorAsync(context,
                StatusCodes.Status500InternalServerError,
                errorResponse);
        }
    }

    private static async Task WriteErrorAsync(
        HttpContext context,
        int statusCode,
        object errorResponse)
    {
        if (context.Response.HasStarted)
            return;
        
        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        
        var payload = JsonSerializer.Serialize(errorResponse);
        await context.Response.WriteAsync(payload);
    }
    
    private static bool TryMapDbUpdate(DbUpdateException ex, out int status, out string message)
    {
        // Postgres unique violation.
        if (ex.InnerException is PostgresException pg)
        {
            // 23505 = unique_violation
            if (pg.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                status = StatusCodes.Status409Conflict;
                message = "Já existe um registro com esses dados.";
                return true;
            }

            // 23503 = foreign_key_violation
            if (pg.SqlState == PostgresErrorCodes.ForeignKeyViolation)
            {
                status = StatusCodes.Status409Conflict;
                message = "Não foi possível concluir por vínculo com outro registro.";
                return true;
            }
        }

        status = 0;
        message = "";
        return false;
    }
}