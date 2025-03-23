using FluxoCaixa.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace FluxoCaixa.Application.Services
{
    [ExcludeFromCodeCoverage]
    public class ServiceFactory(IServiceProvider serviceProvider) : IServiceFactory
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public ILaunchService CreateLaunchService()
        {
            return _serviceProvider.GetRequiredService<ILaunchService>();
        }

        public IConsolidationService CreateConsolidationService()
        {
            return _serviceProvider.GetRequiredService<IConsolidationService>();
        }
    }
}
