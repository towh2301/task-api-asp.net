using System.Net;
using System.Text.Json;
using Controllers.TaskController;

namespace Middleware.ErrorHandlingMiddleware
{
    public class ErrorHandlingMiddleware
    {
        // RequestDelegate is a delegate that represents the next middleware in the pipeline
        private readonly RequestDelegate _next;

        // ILogger is a generic interface for logging 
        // where the category name is derived from the specified TCategoryName type name
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        // Invoke method is called by the middleware pipeline
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return context.Response.WriteAsync(new ErrorDetails()
            {
                StatusCode = context.Response.StatusCode,
                Message = "An error occurred while processing your request."
            }.ToString());
        }

        public class ErrorDetails
        {
            public int StatusCode { get; set; }
            public string? Message { get; set; }

            public override string ToString()
            {
                return JsonSerializer.Serialize(this);
            }
        }
    }
}