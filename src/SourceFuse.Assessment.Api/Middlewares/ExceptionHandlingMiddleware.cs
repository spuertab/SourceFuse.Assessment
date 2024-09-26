using System.Net;
using System.Text.Json;

namespace SourceFuse.Assessment.Api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                // Check for specific status codes and log them
                if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("Unauthorized access attempt to {Path}", context.Request.Path);
                }
                else if (context.Response.StatusCode == (int)HttpStatusCode.BadRequest)
                {
                    _logger.LogWarning("Bad request made to {Path}", context.Request.Path);
                }
                else if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
                {
                    _logger.LogWarning("Forbidden request made to {Path}", context.Request.Path);
                }
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "An argument exception has occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex, HttpStatusCode.BadRequest);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "An invalid operation exception has occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex, HttpStatusCode.Conflict);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception has occurred.");
                await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception, HttpStatusCode statusCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var result = JsonSerializer.Serialize(new
            {
                context.Response.StatusCode,
                exception.Message,
                Detailed = exception.ToString()
            });

            return context.Response.WriteAsync(result);
        }
    }
}