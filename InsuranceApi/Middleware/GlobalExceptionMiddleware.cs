using InsuranceApi.DTO;
using System.Text.Json;

namespace InsuranceApi.Middleware
{
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
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status400BadRequest;

                var response = new ErrorResponseDTO
                {
                    StatusCode = context.Response.StatusCode,
                    ErrorType = "Bad Request",
                    Message = ex.Message,
                    RequestPath = context.Request.Path
                };

                _logger.LogError(ex,"Unhandled exception occurred while processing {Path}",context.Request.Path);

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
