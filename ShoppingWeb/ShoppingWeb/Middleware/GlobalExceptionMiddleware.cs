using ShoppingWeb.Response;
using System.Net;

namespace ShoppingWeb.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;


    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
        catch (Exception e)
        {
            _logger.LogError(e, $"An unhandled exception occurred. Message: {e.Message}, Source: {e.Source}");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // Internal Server Error
            context.Response.ContentType = "application/json";

            var response = ApiResponse<string>.ErrorResponse(e.Message);
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}