using System.Diagnostics.CodeAnalysis;

namespace FluxoCaixa.API.Middlewares
{
    /// <summary>
    /// Middleware de rastreabilidade do fluxo
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="next"></param>
    [ExcludeFromCodeCoverage]
    public class CorrelationMiddleware(ILogger<CorrelationMiddleware> logger, RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<CorrelationMiddleware> _logger = logger;

        /// <summary>
        /// Invoke
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.ContainsKey("X-Correlation-ID"))
                context.Request.Headers["X-Correlation-ID"] = Guid.NewGuid().ToString();

            var correlationId = context.Request.Headers["X-Correlation-ID"].ToString();
            context.Response.Headers["X-Correlation-ID"] = correlationId;

            _logger.LogInformation("Requisição iniciando com Correlation ID {correlationId}.", correlationId);

            await _next(context);

            _logger.LogInformation("Requisição finalizando com Correlation ID {correlationId}.", correlationId);
        }
    }

    /// <summary>
    /// Classe de extensão para adicionar o middleware de rastreabilidade
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class CorrelationIdMiddlewareExtensions
    {
        /// <summary>
        /// Adiciona extensão do middleware de rastreabilidade
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CorrelationMiddleware>();
        }
    }
}
