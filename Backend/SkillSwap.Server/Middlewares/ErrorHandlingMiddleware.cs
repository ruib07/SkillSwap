using SkillSwap.Services.Helpers;
using System.Net;
using static SkillSwap.Server.Models.Responses;

namespace SkillSwap.Server.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (CustomException ex)
        {
            _logger.LogError($"Custom Error: {ex.Message}");
            await HandleExceptionAsync(httpContext, ex, ex.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unhandled Error: {ex.Message}");
            await HandleExceptionAsync(httpContext, ex, HttpStatusCode.InternalServerError);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception, HttpStatusCode statusCode)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var errorResponse = new ErrorResponse()
        {
            Message = exception.Message,
            StatusCode = context.Response.StatusCode
        };

        return context.Response.WriteAsJsonAsync(errorResponse);
    }
}
