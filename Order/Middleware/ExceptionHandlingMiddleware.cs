using Order.DTO;
using System.Net;

namespace Order.Middleware
{
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
                context.Response.StatusCode =
                    StatusCodes.Status500InternalServerError;

                context.Response.ContentType =
                    "application/json";

                _logger.LogError(ex, "An unhandled exception occurred.");

                var response = new ApiResponse<object>
                {
                    Success = false,
                    Message = "An unexpected error occurred.",
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Data = null
                };


                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}