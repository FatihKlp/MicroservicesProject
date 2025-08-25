using System.Net;
using Shared.DTOs;
using Shared.Interfaces;

namespace AuthService.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogServiceClient _logService;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogServiceClient logService)
        {
            _next = next;
            _logService = logService;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Hata logunu LogService'e gönder
                await _logService.SendLogAsync(new LogEntryDto
                {
                    Service = "ProductService",
                    Level = "Error",
                    Message = $"{ex.Message} | {ex.StackTrace}",
                    UserId = context.User?.Identity?.Name
                });

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsJsonAsync(new { error = "Something went wrong!" });
            }
        }
    }

    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
