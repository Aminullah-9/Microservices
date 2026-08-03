using ProductService.DTO;

namespace ProductService.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
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


                var response = new ApiResponse<object>
                {
                    Success = false,
                    Message = "An unexpected error occurred.",
                    Data = null
                };


                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }}
