using FluxoCaixa.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FluxoCaixa.Application.Services
{
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
