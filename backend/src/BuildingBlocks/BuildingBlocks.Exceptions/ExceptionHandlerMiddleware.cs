using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Exceptions;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
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
            _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, response) = exception switch
        {
            NotFoundException notFound => (StatusCodes.Status404NotFound, new ErrorResponse
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = notFound.Message
            }),
            BadRequestException badRequest => (StatusCodes.Status400BadRequest, new ErrorResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = badRequest.Message,
                Details = badRequest.Details
            }),
            InternalServerException internalServer => (StatusCodes.Status500InternalServerError, new ErrorResponse
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = internalServer.Message,
                Details = internalServer.Details
            }),
            _ => (StatusCodes.Status500InternalServerError, new ErrorResponse
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An unexpected error occurred."
            })
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string[]? Details { get; set; }
}
