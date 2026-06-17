using Domain.Dtos;
using Domain.Exceptions;
using FluentValidation;
using System.Text.Json;

namespace Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            _logger.LogError(ex, "Unhandled exception");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, code, message, details) = ex switch
        {
            NotFoundException nfe => (StatusCodes.Status404NotFound, "NOT_FOUND", nfe.Message, (IReadOnlyList<string>?)null),
            ConflictException ce => (StatusCodes.Status409Conflict, "CONFLICT", ce.Message, null),
            ValidationException ve => (
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "One or more validation errors occurred.",
                (IReadOnlyList<string>?)ve.Errors.Select(e => e.ErrorMessage).ToList()
            ),
            _ => (StatusCodes.Status500InternalServerError, "INTERNAL_ERROR", "An unexpected error occurred.", null)
        };

        context.Response.StatusCode = statusCode;

        var response = new ErrorResponse(new ErrorDetail(code, message, details));

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        }));
    }
}
