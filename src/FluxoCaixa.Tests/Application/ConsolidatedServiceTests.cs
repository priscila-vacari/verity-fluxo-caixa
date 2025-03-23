using AutoFixture;
using AutoMapper;
using FluxoCaixa.Application.DTOs;
using FluxoCaixa.Application.Interfaces;
using FluxoCaixa.Application.Services;
using FluxoCaixa.Domain.Entities;
using FluxoCaixa.Domain.Enum;
using FluxoCaixa.Domain.Exceptions;
using FluxoCaixa.Infra.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FluxoCaixa.Tests.Application
{
    public class ConsolidatedServiceTests
    {
        private readonly Mock<ILogger<ConsolidationService>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILaunchService> _launchServiceMock;
        private readonly Mock<IRepository<Consolidation>> _repositoryMock;
        private readonly Mock<IServiceProvider> _serviceProviderMock;
        private readonly Mock<IServiceScope> _serviceScopeMock;
        private readonly Mock<IServiceScopeFactory> _scopeFactoryMock;
        private readonly IConsolidationService _consolidationService;

        public ConsolidatedServiceTests()
        {
            _loggerMock = new Mock<ILogger<ConsolidationService>>();
            _mapperMock = new Mock<IMapper>();
            _launchServiceMock = new Mock<ILaunchService>(); 
            _repositoryMock = new Mock<IRepository<Consolidation>>();
            _serviceProviderMock = new Mock<IServiceProvider>();
            _serviceScopeMock = new Mock<IServiceScope>();
            _scopeFactoryMock = new Mock<IServiceScopeFactory>();

            _serviceScopeMock.Setup(s => s.ServiceProvider).Returns(_serviceProviderMock.Object);
            _scopeFactoryMock.Setup(f => f.CreateScope()).Returns(_serviceScopeMock.Object);

            _serviceProviderMock.Setup(p => p.GetService(typeof(IServiceScopeFactory))).Returns(_scopeFactoryMock.Object);
            _serviceProviderMock.Setup(p => p.GetService(typeof(IRepository<Consolidation>))).Returns(_repositoryMock.Object);

            _consolidationService = new ConsolidationService(_loggerMock.Object, _mapperMock.Object, _launchServiceMock.Object, _serviceProviderMock.Object);
        }

        [Theory]
        [InlineData("2025-03-21")]
        public async Task GetByDateAsync_ShouldCallGetByKeysAsyncRepository(DateTime date)
        {
            var consolidation = new Fixture().Create<Consolidation>();
            _repositoryMock.Setup(r => r.GetByKeysAsync(It.IsAny<DateTime>())).ReturnsAsync(consolidation);

            var consolidationDTO = new Fixture().Create<ConsolidationDTO>();
            _mapperMock.Setup(m => m.Map<ConsolidationDTO>(It.IsAny<Consolidation>())).Returns(consolidationDTO);

            var response = await _consolidationService.GetByDateAsync(date);

            Assert.Equal(consolidationDTO, response);
            _repositoryMock.Verify(r => r.GetByKeysAsync(It.IsAny<DateTime>()), Times.Once);
            _mapperMock.Verify(r => r.Map<ConsolidationDTO>(It.IsAny<Consolidation>()), Times.Once);
        }

        [Theory]
        [InlineData("2025-03-21")]
        public async Task GetByDateAsync_ShouldHandleNotFoundException(DateTime date)
        {
            Consolidation? consolidation = null;
            _repositoryMock.Setup(r => r.GetByKeysAsync(It.IsAny<DateTime>())).ReturnsAsync(consolidation);

            var ex = await Assert.ThrowsAsync<NotFoundException>(() => _consolidationService.GetByDateAsync(date));

            Assert.Equal("Consolidação não encontrada para esta data.", ex.Message);

            _repositoryMock.Verify(r => r.GetByKeysAsync(It.IsAny<DateTime>()), Times.Once);
            _mapperMock.Verify(r => r.Map<ConsolidationDTO>(It.IsAny<Consolidation>()), Times.Never);
        }

        [Theory]
        [InlineData("2025-03-01", "2025-03-31")]
        public async Task GetByRangeDateAsync_ShouldCallGetWhereAsyncRepository(DateTime dateStart, DateTime dateEnd)
        {
            var consolidations = new Fixture().Create<IEnumerable<Consolidation>>();
            _repositoryMock.Setup(r => r.GetWhereAsync(It.IsAny<Expression<Func<Consolidation, bool>>>())).ReturnsAsync(consolidations);

            var consolidationsDTO = new Fixture().Create<IEnumerable<ConsolidationDTO>>();
            _mapperMock.Setup(m => m.Map<IEnumerable<ConsolidationDTO>>(It.IsAny<IEnumerable<Consolidation>>())).Returns(consolidationsDTO);

            var response = await _consolidationService.GetByRangeDateAsync(dateStart, dateEnd);

            Assert.Equal(consolidationsDTO, response);
            _repositoryMock.Verify(r => r.GetWhereAsync(It.IsAny<Expression<Func<Consolidation, bool>>>()), Times.Once);
            _mapperMock.Verify(r => r.Map<IEnumerable<ConsolidationDTO>>(It.IsAny<IEnumerable<Consolidation>>()), Times.Once);
        }

        [Theory]
        [InlineData("2025-03-21")]
        public async Task GenerateDailyconsolidationAsync_WhenFoundLaunches_ShouldCallAddAsync(DateTime date)
        {
            var launches = new Fixture().Create<IEnumerable<LaunchDTO>>();
            var consolidation = new Fixture().Create<ConsolidationDTO>();

            _launchServiceMock.Setup(r => r.GetByDateAsync(It.IsAny<DateTime>())).ReturnsAsync(launches);
            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Consolidation>())).Returns(Task.CompletedTask);
            _mapperMock.Setup(r => r.Map<ConsolidationDTO>(It.IsAny<Consolidation>())).Returns(consolidation);

            var response = await _consolidationService.GenerateDailyconsolidationAsync(date);

            Assert.NotNull(response);
            _launchServiceMock.Verify(r => r.GetByDateAsync(It.IsAny<DateTime>()), Times.Once);
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Consolidation>()), Times.Once);
            _mapperMock.Verify(r => r.Map<ConsolidationDTO>(It.IsAny<Consolidation>()), Times.Once);
        }
    }
}