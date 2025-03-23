using AutoFixture;
using AutoMapper;
using FluxoCaixa.API.Controllers.v1;
using FluxoCaixa.API.Models;
using FluxoCaixa.Application.DTOs;
using FluxoCaixa.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace FluxoCaixa.Tests.API.Controllers
{
    public class ConsolidationControllerTests
    {
        private readonly Mock<ILogger<LaunchController>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IServiceFactory> _serviceFactoryMock;
        private readonly Mock<IConsolidationService> _consolidationServiceMock;
        private readonly ConsolidationController _serviceController;

        public ConsolidationControllerTests()
        {
            _loggerMock = new Mock<ILogger<LaunchController>>();
            _mapperMock = new Mock<IMapper>();
            _serviceFactoryMock = new Mock<IServiceFactory>();
            _consolidationServiceMock = new Mock<IConsolidationService>();

            _serviceFactoryMock.Setup(f => f.CreateConsolidationService()).Returns(_consolidationServiceMock.Object);

            _serviceController = new ConsolidationController(_loggerMock.Object, _mapperMock.Object, _serviceFactoryMock.Object);
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