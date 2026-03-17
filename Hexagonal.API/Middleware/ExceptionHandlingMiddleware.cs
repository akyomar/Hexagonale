using System.Net;
using System.Text.Json;
using Hexagonal.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Hexagonal.API.Middleware;

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
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Erreur non gérée : {Message}", exception.Message);

        var (statusCode, problem) = exception switch
        {
            NotFoundException nf => ((int)HttpStatusCode.NotFound, new ProblemDetails
            {
                Title = "Ressource non trouvée",
                Detail = nf.Message,
                Status = (int)HttpStatusCode.NotFound,
                Extensions =
                {
                    ["code"] = nf.Code,
                    ["entity"] = nf.EntityName
                }
            }),

            DomainException d when d.Code == "VALIDATION" => ((int)HttpStatusCode.BadRequest, new ProblemDetails
            {
                Title = "Erreur de validation",
                Detail = d.Message,
                Status = (int)HttpStatusCode.BadRequest,
                Extensions =
                {
                    ["code"] = d.Code
                }
            }),

            DomainException d => (StatusCodes.Status422UnprocessableEntity, new ProblemDetails
            {
                Title = "Erreur métier",
                Detail = d.Message,
                Status = StatusCodes.Status422UnprocessableEntity,
                Extensions =
                {
                    ["code"] = d.Code
                }
            }),

            ArgumentException arg => ((int)HttpStatusCode.BadRequest, new ProblemDetails
            {
                Title = "Requête invalide",
                Detail = arg.Message,
                Status = (int)HttpStatusCode.BadRequest
            }),

            _ => ((int)HttpStatusCode.InternalServerError, new ProblemDetails
            {
                Title = "Erreur interne",
                Detail = "Une erreur inattendue s'est produite.",
                Status = (int)HttpStatusCode.InternalServerError
            })
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(
                problem,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }));
    }
}