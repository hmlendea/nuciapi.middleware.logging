using Microsoft.AspNetCore.Builder;

namespace NuciAPI.Middleware.Logging
{
    public static class NuciApiSecurityExtensions
    {
        public static IApplicationBuilder UseNuciApiRequestLogging(
            this IApplicationBuilder app)
            => app.UseMiddleware<RequestLoggingMiddleware>();
    }
}