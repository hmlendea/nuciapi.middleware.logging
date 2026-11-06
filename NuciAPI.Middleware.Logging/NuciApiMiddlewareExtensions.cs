using Microsoft.AspNetCore.Builder;

namespace NuciAPI.Middleware.Logging
{
    public static class NuciApiMiddlewareExtensions
    {
        /// <summary>
        /// Adds the NuciAPI request logging middleware to the application pipeline.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <returns>The application builder for chaining.</returns>
        public static IApplicationBuilder UseNuciApiRequestLogging(
            this IApplicationBuilder app)
            => app.UseMiddleware<RequestLoggingMiddleware>();
    }
}