using PetFamily.Api.Response;

namespace PetFamily.Api.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware
    (RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        _logger.LogError(exception, "An unexpected error occurred.");

        var (statuscode, responseError) = exception switch
        {
            UnauthorizedAccessException _ => (
                StatusCodes.Status401Unauthorized,
                new ResponseError("user.not.authorized", exception.Message, null)),
            _ => (
                StatusCodes.Status500InternalServerError,
                new ResponseError("internal.server.error", exception.Message, null))
        };
        var envelope = Envelope.Errors([responseError]);

        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = statuscode;

        await httpContext.Response.WriteAsJsonAsync(envelope);
    }
}

public static class ExceptionHandling
{
    public static void UseExceptionHandling(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}