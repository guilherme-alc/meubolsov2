using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace MeuBolso.API.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
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
        catch (DbUpdateConcurrencyException)
        {
            await WriteErrorAsync(context, 
                StatusCodes.Status409Conflict, 
                "Conflito de concorrência. Tente novamente.");
        }
        catch (DbUpdateException ex) when (TryMapDbUpdate(ex, out var status, out var message))
        {
            await WriteErrorAsync(context, status, message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado");
            
            await WriteErrorAsync(context,
                StatusCodes.Status500InternalServerError,
                "Erro interno no servidor");
        }
    }

    private static async Task WriteErrorAsync(
        HttpContext context,
        int statusCode,
        string message)
    {
        if (context.Response.HasStarted)
            return;
        
        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        
        var payload = JsonSerializer.Serialize(new { error = message });
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