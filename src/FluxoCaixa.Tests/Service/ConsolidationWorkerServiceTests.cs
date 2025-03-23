using FluxoCaixa.Application.Interfaces;
using FluxoCaixa.Service.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace FluxoCaixa.Tests.Service
{
    public class ConsolidationWorkerServiceTests
    {

        private readonly Mock<ILogger<ConsolidationWorkerService>> _loggerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IServiceFactory> _serviceFactoryMock;
        private readonly Mock<IConsolidationService> _consolidationServiceMock;
        private readonly ConsolidationWorkerService _consolidationWorker;

        public ConsolidationWorkerServiceTests()
        {
            _loggerMock = new Mock<ILogger<ConsolidationWorkerService>>();
            _configurationMock = new Mock<IConfiguration>();
            _serviceFactoryMock = new Mock<IServiceFactory>();
            _consolidationServiceMock = new Mock<IConsolidationService>();

            _serviceFactoryMock.Setup(f => f.CreateConsolidationService()).Returns(_consolidationServiceMock.Object);

            _configurationMock.Setup(c => c["Frequency"]).Returns("24");

            _consolidationWorker = new ConsolidationWorkerService(_loggerMock.Object, _serviceFactoryMock.Object, _configurationMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldCallGenerateDailyconsolidationAsync()
        {
            using var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(5));

            await _consolidationWorker.StartAsync(cts.Token);
            await Task.Delay(1500);

            _consolidationServiceMock.Verify(service => service.GenerateDailyconsolidationAsync(It.IsAny<DateTime>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task ExecuteAsync_WithoutConfigRequency_ShouldCallGenerateDailyconsolidationAsync()
        {
            var mockConfiguration = new Mock<IConfiguration>();
            var consolidationWorker = new ConsolidationWorkerService(_loggerMock.Object, _serviceFactoryMock.Object, mockConfiguration.Object);

            using var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(5));

            await consolidationWorker.StartAsync(cts.Token);
            await Task.Delay(1500);

            _consolidationServiceMock.Verify(service => service.GenerateDailyconsolidationAsync(It.IsAny<DateTime>()), Times.AtLeastOnce);
        }


        [Fact]
        public async Task ExecuteAsync_ShouldHandleCancellationGracefully()
        {
            using var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMilliseconds(100));

            await Task.Delay(200);
            await _consolidationWorker.StartAsync(cts.Token);

            _consolidationServiceMock.Verify(service => service.GenerateDailyconsolidationAsync(It.IsAny<DateTime>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldHandleExceptionDuringConsolidation()
        {
            var mockServiceFactory = new Mock<IServiceFactory>();
            var mockConsolidationService = new Mock<IConsolidationService>();

            mockConsolidationService
                .Setup(service => service.GenerateDailyconsolidationAsync(It.IsAny<DateTime>()))
                .Throws(new InvalidOperationException("Simulated exception during consolidation"));

            mockServiceFactory
                .Setup(factory => factory.CreateConsolidationService())
                .Returns(mockConsolidationService.Object);

            var hostedService = new ConsolidationWorkerService(_loggerMock.Object, mockServiceFactory.Object, _configurationMock.Object);

            using var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(5));

            await hostedService.StartAsync(cts.Token);
            await Task.Delay(1500);

            mockConsolidationService.Verify(service => service.GenerateDailyconsolidationAsync(It.IsAny<DateTime>()), Times.AtLeastOnce);
            mockServiceFactory.Verify(factory => factory.CreateConsolidationService(), Times.AtLeastOnce);
        }
    }
}
