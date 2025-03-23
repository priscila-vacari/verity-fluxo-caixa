using FluxoCaixa.Application.Interfaces;
using FluxoCaixa.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace FluxoCaixa.Application
{
    [ExcludeFromCodeCoverage]
    public static class ApplicationDependencyRegister
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddDependencyServices();
        }

        private static IServiceCollection AddDependencyServices(this IServiceCollection services) 
        {
            services.AddSingleton<IServiceFactory, ServiceFactory>();
            services.AddTransient<ILaunchService, LaunchService>();
            services.AddTransient<IConsolidationService, ConsolidationService>();

            return services;
        }
    }
}
