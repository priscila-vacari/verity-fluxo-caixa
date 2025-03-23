using AutoFixture;
using AutoMapper;
using FluxoCaixa.API.Controllers.v1;
using FluxoCaixa.API.Models;
using FluxoCaixa.Application.DTOs;
using FluxoCaixa.Application.Interfaces;
using FluxoCaixa.Domain.Entities;
using FluxoCaixa.Domain.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;

namespace FluxoCaixa.Tests.API.Controllers
{
    public class FinancialControllerTests
    {
        private readonly Mock<ILogger<FinancialController>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IServiceFactory> _serviceFactoryMock;
        private readonly Mock<ILaunchService> _launchServiceMock;
        private readonly Mock<IConsolidationService> _consolidationServiceMock;
        private readonly FinancialController _serviceController;

        public FinancialControllerTests()
        {
            _loggerMock = new Mock<ILogger<FinancialController>>();
            _mapperMock = new Mock<IMapper>();
            _serviceFactoryMock = new Mock<IServiceFactory>();
            _launchServiceMock = new Mock<ILaunchService>();
            _consolidationServiceMock = new Mock<IConsolidationService>();

            _serviceFactoryMock.Setup(f => f.CreateLaunchService()).Returns(_launchServiceMock.Object);
            _serviceFactoryMock.Setup(f => f.CreateConsolidationService()).Returns(_consolidationServiceMock.Object);

            _serviceController = new FinancialController(_loggerMock.Object, _mapperMock.Object, _serviceFactoryMock.Object);
        }

        [Theory]
        [InlineData("2025-03-21")]
        public async Task GetLaunchByDate_ReturnsOkResult_WithData(DateTime date)
        {
            var launches = new Fixture().Create<IEnumerable<LaunchDTO>>();
            _launchServiceMock.Setup(r => r.GetByDateAsync(It.IsAny<DateTime>())).ReturnsAsync(launches);

            var launchesModel = new Fixture().Create<IEnumerable<LaunchResponseModel>>();
            _mapperMock.Setup(r => r.Map<IEnumerable<LaunchResponseModel>>(It.IsAny<IEnumerable<LaunchDTO>>())).Returns(launchesModel);

            var result = await _serviceController.GetLaunchByDate(date);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(launchesModel, okResult.Value);

            _launchServiceMock.Verify(r => r.GetByDateAsync(It.IsAny<DateTime>()), Times.Once);
            _mapperMock.Verify(r => r.Map<IEnumerable<LaunchResponseModel>>(It.IsAny<IEnumerable<LaunchDTO>>()), Times.Once);
        }

        [Fact]
        public async Task AddLaunch_ReturnsCreatedAtAction_WhenValidLaunch()
        {
            var launchRequestModel = new LaunchRequestModel { Date = DateTime.Today, Amount = 100, Type = LaunchType.Credit };
            var launchResponseModel = new LaunchResponseModel { Date = DateTime.Today, Amount = 100, Type = LaunchType.Credit };
            var launchDto = new LaunchDTO { Date = DateTime.Today, Amount = 100, Type = LaunchType.Credit };

            _mapperMock.Setup(r => r.Map<LaunchDTO>(It.IsAny<LaunchRequestModel>())).Returns(launchDto);
            _mapperMock.Setup(r => r.Map<LaunchResponseModel>(It.IsAny<LaunchDTO>())).Returns(launchResponseModel);
            _launchServiceMock.Setup(r => r.AddAsync(It.IsAny<LaunchDTO>())).Returns(Task.CompletedTask);

            var result = await _serviceController.AddLaunch(launchRequestModel);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(launchResponseModel, createdResult.Value);

            _launchServiceMock.Verify(r => r.AddAsync(It.IsAny<LaunchDTO>()), Times.Once);
            _mapperMock.Verify(r => r.Map<LaunchDTO>(It.IsAny<LaunchRequestModel>()), Times.Once);
            _mapperMock.Verify(r => r.Map<LaunchResponseModel>(It.IsAny<LaunchDTO>()), Times.Once);
        }

        [Theory]
        [InlineData("2025-03-21")]
        public async Task GetConsolidationByDate_ReturnsOkResult_WithData(DateTime date)
        {
            var consolidation = new Fixture().Create<ConsolidationDTO>();
            _consolidationServiceMock.Setup(r => r.GetByDateAsync(It.IsAny<DateTime>())).ReturnsAsync(consolidation);

            var consolidationModel = new Fixture().Create<ConsolidationResponseModel>();
            _mapperMock.Setup(r => r.Map<ConsolidationResponseModel>(It.IsAny<ConsolidationDTO>())).Returns(consolidationModel);

            var result = await _serviceController.GetConsolidationByDate(date);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(consolidationModel, okResult.Value);

            _consolidationServiceMock.Verify(r => r.GetByDateAsync(It.IsAny<DateTime>()), Times.Once);
            _mapperMock.Verify(r => r.Map<ConsolidationResponseModel>(It.IsAny<ConsolidationDTO>()), Times.Once);
        }

        [Theory]
        [InlineData("2025-03-01", "2025-03-31")]
        public async Task GetConsolidationByRangeDate_ReturnsOkResult_WithData(DateTime dateStart, DateTime dateEnd)
        {
            var consolidations = new Fixture().Create<IEnumerable<ConsolidationDTO>>();
            _consolidationServiceMock.Setup(r => r.GetByRangeDateAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(consolidations);

            var consolidationsModel = new Fixture().Create<IEnumerable<ConsolidationResponseModel>>();
            _mapperMock.Setup(r => r.Map<IEnumerable<ConsolidationResponseModel>>(It.IsAny<IEnumerable<ConsolidationDTO>>())).Returns(consolidationsModel);

            var request = new ConsolidationRequestModel { DateStart = dateStart, DateEnd = dateEnd };
            var result = await _serviceController.GetConsolidationByRangeDate(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(consolidationsModel, okResult.Value);

            _consolidationServiceMock.Verify(r => r.GetByRangeDateAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
            _mapperMock.Verify(r => r.Map<IEnumerable<ConsolidationResponseModel>>(It.IsAny<IEnumerable<ConsolidationDTO>>()), Times.Once);
        }

        [Theory]
        [InlineData("2025-03-21")]
        public async Task GenerateDailyconsolidationAsync_ReturnsCreatedAtAction_WithData(DateTime date)
        {
            var consolidation = new Fixture().Create<ConsolidationDTO>();
            _consolidationServiceMock.Setup(r => r.GenerateDailyconsolidationAsync(It.IsAny<DateTime>())).ReturnsAsync(consolidation);

            var consolidationModel = new Fixture().Create<ConsolidationResponseModel>();
            _mapperMock.Setup(r => r.Map<ConsolidationResponseModel>(It.IsAny<ConsolidationDTO>())).Returns(consolidationModel);

            var result = await _serviceController.GenerateDailyconsolidationAsync(date);

            var createdResult = Assert.IsType<CreatedResult>(result);
            Assert.Equal(consolidationModel, createdResult.Value);

            _consolidationServiceMock.Verify(r => r.GenerateDailyconsolidationAsync(It.IsAny<DateTime>()), Times.Once);
            _mapperMock.Verify(r => r.Map<ConsolidationResponseModel>(It.IsAny<ConsolidationDTO>()), Times.Once);
        }
    }
}