using FluxoCaixa.Application.Interfaces;
using FluxoCaixa.Application.Services;
using NSubstitute;

namespace FluxoCaixa.Tests.Application.Services
{
    public class ServiceFactoryTests
    {
        [Fact]
        public void Deve_Retornar_ILaunchService_Corretamente()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            var mockLaunchService = Substitute.For<ILaunchService>();

            serviceProvider.GetService(typeof(ILaunchService)).Returns(mockLaunchService);
            var serviceFactory = new ServiceFactory(serviceProvider);

            var launchService = serviceFactory.CreateLaunchService();

            Assert.NotNull(launchService);
            Assert.Equal(mockLaunchService, launchService);
        }

        [Fact]
        public void Deve_Retornar_IConsolidationService_Corretamente()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            var mockConsolidationService = Substitute.For<IConsolidationService>();

            serviceProvider.GetService(typeof(IConsolidationService)).Returns(mockConsolidationService);
            var serviceFactory = new ServiceFactory(serviceProvider);

            var consolidationService = serviceFactory.CreateConsolidationService();

            Assert.NotNull(consolidationService);
            Assert.Equal(mockConsolidationService, consolidationService);
        }
    }
}
