using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Saldoa.API.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    private bool IsDevelopment => _env.IsDevelopment();

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
            if (!context.Response.HasStarted)
                context.Response.StatusCode = 499;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Conflito de concorrência ao persistir dados");

            await WriteProblemAsync(
                context,
                StatusCodes.Status409Conflict,
                title: "Database.ConcurrencyConflict",
                detail: IsDevelopment
                    ? $"Conflito de concorrência. Tente novamente: {ex.Message}"
                    : "Conflito de concorrência. Tente novamente.");
        }
        catch (DbUpdateException ex) when (TryMapDbUpdate(ex, out var statusCode, out var title, out var detail))
        {
            _logger.LogError(ex, "Erro ao persistir dados");

            await WriteProblemAsync(
                context,
                statusCode,
                title,
                IsDevelopment ? $"{detail} {ex.Message}" : detail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado");

            await WriteProblemAsync(
                context,
                StatusCodes.Status500InternalServerError,
                title: "Server.UnexpectedError",
                detail: IsDevelopment ? ex.Message : "Erro interno no servidor");
        }
    }

    private static async Task WriteProblemAsync(
        HttpContext context,
        int statusCode,
        string title,
        string detail)
    {
        if (context.Response.HasStarted)
            return;

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        await context.Response.WriteAsJsonAsync(problem);
    }

    private static bool TryMapDbUpdate(
        DbUpdateException ex,
        out int statusCode,
        out string title,
        out string detail)
    {
        if (ex.InnerException is PostgresException pg)
        {
            if (pg.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                statusCode = StatusCodes.Status409Conflict;
                title = "Database.UniqueViolation";
                detail = "Já existe um registro com esses dados.";
                return true;
            }

            if (pg.SqlState == PostgresErrorCodes.ForeignKeyViolation)
            {
                statusCode = StatusCodes.Status409Conflict;
                title = "Database.ForeignKeyViolation";
                detail = "Não foi possível concluir por vínculo com outro registro.";
                return true;
            }
        }

        statusCode = 0;
        title = "";
        detail = "";
        return false;
    }
}