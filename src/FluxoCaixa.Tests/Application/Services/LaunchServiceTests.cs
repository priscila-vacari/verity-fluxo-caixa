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

namespace FluxoCaixa.Tests.Application.Services
{
    public class LaunchServiceTests
    {
        private readonly Mock<ILogger<LaunchService>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepository<Launch>> _repositoryMock;
        private readonly Mock<IServiceProvider> _serviceProviderMock;
        private readonly Mock<IServiceScope> _serviceScopeMock;
        private readonly Mock<IServiceScopeFactory> _scopeFactoryMock;
        private readonly ILaunchService _launchService;

        public LaunchServiceTests()
        {
            _loggerMock = new Mock<ILogger<LaunchService>>();
            _mapperMock = new Mock<IMapper>();
            _repositoryMock = new Mock<IRepository<Launch>>();
            _serviceProviderMock = new Mock<IServiceProvider>();
            _serviceScopeMock = new Mock<IServiceScope>();
            _scopeFactoryMock = new Mock<IServiceScopeFactory>();

            _serviceScopeMock.Setup(s => s.ServiceProvider).Returns(_serviceProviderMock.Object);
            _scopeFactoryMock.Setup(f => f.CreateScope()).Returns(_serviceScopeMock.Object);

            _serviceProviderMock.Setup(p => p.GetService(typeof(IServiceScopeFactory))).Returns(_scopeFactoryMock.Object);
            _serviceProviderMock.Setup(p => p.GetService(typeof(IRepository<Launch>))).Returns(_repositoryMock.Object);

            _launchService = new LaunchService(_loggerMock.Object, _mapperMock.Object, _serviceProviderMock.Object);
        }

        [Theory]
        [InlineData("2025-03-21")]
        public async Task GetByDateAsync_ShouldCallGetWhereAsyncRepository(DateTime date)
        {
            var launches = new Fixture().Create<IEnumerable<Launch>>();
            _repositoryMock.Setup(r => r.GetWhereAsync(It.IsAny<Expression<Func<Launch, bool>>>())).ReturnsAsync(launches);

            var launchesDto = new Fixture().Create<IEnumerable<LaunchDTO>>();
            _mapperMock.Setup(m => m.Map<IEnumerable<LaunchDTO>>(It.IsAny<IEnumerable<Launch>>())).Returns(launchesDto);

            var response = await _launchService.GetByDateAsync(date);

            Assert.Equal(launches.Count(), response.Count());
            _repositoryMock.Verify(r => r.GetWhereAsync(It.IsAny<Expression<Func<Launch, bool>>>()), Times.Once);
            _mapperMock.Verify(r => r.Map<IEnumerable<LaunchDTO>>(It.IsAny<IEnumerable<Launch>>()), Times.Once);
        }

        [Theory]
        [InlineData("2025-03-21")]
        public async Task GetByDateAsync_ShouldHandleNotFoundException(DateTime date)
        {
            IEnumerable<Launch> launches = new List<Launch>();
            _repositoryMock.Setup(r => r.GetWhereAsync(It.IsAny<Expression<Func<Launch, bool>>>())).ReturnsAsync(launches);

            var ex = await Assert.ThrowsAsync<NotFoundException>(() => _launchService.GetByDateAsync(date));

            Assert.Equal("Lançamento não encontrado para a data.", ex.Message);

            _repositoryMock.Verify(r => r.GetWhereAsync(It.IsAny<Expression<Func<Launch, bool>>>()), Times.Once);
            _mapperMock.Verify(r => r.Map<ConsolidationDTO>(It.IsAny<Consolidation>()), Times.Never);
        }


        [Fact]
        public async Task AddAsync_ShouldCallAddAsync()
        {
            var launch = new LaunchDTO { Date = DateTime.Today, Amount = 100, Type = LaunchType.Credit };

            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Launch>())).Returns(Task.CompletedTask);

            Launch? launchExists = null;
            _repositoryMock.Setup(r => r.GetByKeysAsync(It.IsAny<object[]>())).ReturnsAsync(launchExists);

            await _launchService.AddAsync(launch);

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Launch>()), Times.Once);
            _repositoryMock.Verify(r => r.GetByKeysAsync(It.IsAny<object[]>()), Times.Once);
        }

        [Fact]
        public async Task AddAsync_ShouldHandleDuplicateEntryException()
        {
            var launch = new LaunchDTO { Date = DateTime.Today, Amount = 100, Type = LaunchType.Credit };

            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Launch>())).Returns(Task.CompletedTask);

            var launchExists = new Launch { Date = DateTime.Today, Amount = 100, Type = LaunchType.Credit };
            _repositoryMock.Setup(r => r.GetByKeysAsync(It.IsAny<object[]>())).ReturnsAsync(launchExists);

            var ex =  await Assert.ThrowsAsync<DuplicateEntryException>(() => _launchService.AddAsync(launch));

            Assert.Equal("Lançamento já cadastrado.", ex.Message);

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Launch>()), Times.Never);
            _repositoryMock.Verify(r => r.GetByKeysAsync(It.IsAny<object[]>()), Times.Once);
        }
    }
}