using FluxoCaixa.Service.Enums;
using FluxoCaixa.Service.Extension;
using FluxoCaixa.Service.Services;
using System.Diagnostics.CodeAnalysis;

namespace FluxoCaixa.Application
{
    [ExcludeFromCodeCoverage]
    public static class ServiceDependencyRegister
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddDependencyServices();
        }

        private static IServiceCollection AddDependencyServices(this IServiceCollection services) 
        {
            _ = Enum.TryParse(Environment.GetEnvironmentVariable("TYPE_SERVICE"), out ServiceType serviceType);

            services.AddHostedServiceWithCondition<ConsolidationWorkerService>(serviceType == ServiceType.DailyConsolidationService);

            return services;
        }
    }
}
