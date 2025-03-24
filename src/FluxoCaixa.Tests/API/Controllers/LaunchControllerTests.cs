using AutoFixture;
using AutoMapper;
using FluxoCaixa.API.Controllers.v1;
using FluxoCaixa.API.Models;
using FluxoCaixa.Application.DTOs;
using FluxoCaixa.Application.Interfaces;
using FluxoCaixa.Domain.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace FluxoCaixa.Tests.API.Controllers
{
    public class LaunchControllerTests
    {
        private readonly Mock<ILogger<LaunchController>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IServiceFactory> _serviceFactoryMock;
        private readonly Mock<ILaunchService> _launchServiceMock;
        private readonly LaunchController _serviceController;

        public LaunchControllerTests()
        {
            _loggerMock = new Mock<ILogger<LaunchController>>();
            _mapperMock = new Mock<IMapper>();
            _serviceFactoryMock = new Mock<IServiceFactory>();
            _launchServiceMock = new Mock<ILaunchService>();

            _serviceFactoryMock.Setup(f => f.CreateLaunchService()).Returns(_launchServiceMock.Object);

            _serviceController = new LaunchController(_loggerMock.Object, _mapperMock.Object, _serviceFactoryMock.Object);
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
    }
}