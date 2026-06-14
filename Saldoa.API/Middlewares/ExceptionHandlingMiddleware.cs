using Microsoft.AspNetCore.Mvc;
using Saldoa.Application.Common.Exceptions;
using Saldoa.Domain.Exceptions;

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
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Violação de regra de domínio");

            await WriteProblemAsync(
                    context,
                    StatusCodes.Status400BadRequest,
                    title: "Domain.ValidationError",
                    detail: ex.Message
            );
        }
        catch (PersistenceConflictException ex)
        {
            _logger.LogWarning(ex, "Conflito ao persistir dados");

            await WriteProblemAsync(
                    context,
                    StatusCodes.Status409Conflict,
                    title: "Persistence.Conflict",
                    detail: ex.Message
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado");

            await WriteProblemAsync(
                context,
                StatusCodes.Status500InternalServerError,
                title: "Server.UnexpectedError",
                detail: IsDevelopment ? ex.Message : "Erro interno no servidor"
            );
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
}