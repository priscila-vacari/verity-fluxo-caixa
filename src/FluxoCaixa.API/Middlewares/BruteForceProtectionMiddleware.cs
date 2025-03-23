using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics.CodeAnalysis;

namespace FluxoCaixa.API.Middlewares
{
    /// <summary>
    /// Middleware de proteção de força bruta 
    /// </summary>
    /// <param name="next"></param>
    /// <param name="cache"></param>
    /// <param name="logger"></param>
    [ExcludeFromCodeCoverage]
    public class BruteForceProtectionMiddleware(RequestDelegate next, IMemoryCache cache, ILogger<BruteForceProtectionMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly IMemoryCache _cache = cache;
        private readonly ILogger<BruteForceProtectionMiddleware> _logger = logger;
        private readonly int _maxAttempts = 5;
        private readonly TimeSpan _lockoutTime = TimeSpan.FromMinutes(10);

        /// <summary>
        /// Invoke 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            var cacheKey = $"FailedAttempts_{ipAddress}";

            if (_cache.TryGetValue(cacheKey, out int attempts) && attempts >= _maxAttempts)
            {
                context.Response.StatusCode = 429;
                _logger.LogWarning("IP {ipAddress} bloqueado por tentativas excessivas.", ipAddress);
                await context.Response.WriteAsync("Too many failed attempts. Try again later.");
                return;
            }

            await _next(context);
        }
    }

    /// <summary>
    /// Classe de extensão para adicionar o middleware de proteção
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class BruteForceProtectionMiddlewareExtensions
    {
        /// <summary>
        /// Adiciona extensão do middleware de proteção
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseBruteForceProtection(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<BruteForceProtectionMiddleware>();
        }
    }
}
