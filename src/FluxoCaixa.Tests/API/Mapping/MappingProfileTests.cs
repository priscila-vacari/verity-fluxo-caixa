using AutoMapper;
using FluentAssertions;
using FluxoCaixa.API.Mapping;
using FluxoCaixa.API.Models;
using FluxoCaixa.Application.DTOs;

namespace FluxoCaixa.Tests.API.Mapping
{
    public class MappingProfileTests
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IMapper _mapper;

        public MappingProfileTests()
        {
            // Arrange: Configura o AutoMapper com o perfil de mapeamento
            _configurationProvider = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            _mapper = _configurationProvider.CreateMapper();
        }

        [Fact]
        public void MappingConfiguration_Deve_Ser_Valida()
        {
            _configurationProvider.AssertConfigurationIsValid();
        }

        [Fact]
        public void Deve_Mapear_LaunchRequestModel_Para_LaunchDTO()
        {
            var requestModel = new LaunchRequestModel
            {
                Date = DateTime.Today,
                Type = Domain.Enum.LaunchType.Credit,
                Amount = 123.45m
            };

            var dto = _mapper.Map<LaunchDTO>(requestModel);

            dto.Should().NotBeNull();
            dto.Date.Should().Be(requestModel.Date);
            dto.Type.Should().Be(requestModel.Type);
            dto.Amount.Should().Be(requestModel.Amount);
        }

        [Fact]
        public void Deve_Mapear_LaunchDTO_Para_LaunchResponseModel()
        {
            var dto = new LaunchDTO
            {
                Date = DateTime.Today,
                Type = Domain.Enum.LaunchType.Credit,
                Amount = 123.45m
            };

            var responseModel = _mapper.Map<LaunchResponseModel>(dto);

            responseModel.Should().NotBeNull();
            responseModel.Date.Should().Be(dto.Date);
            responseModel.Type.Should().Be(dto.Type);
            responseModel.Amount.Should().Be(dto.Amount);
        }

        [Fact]
        public void Deve_Mapear_ConsolidationResponseModel_Para_ConsolidationDTO()
        {
            var responseModel = new ConsolidationResponseModel
            {
                Date = DateTime.Today,
                TotalCredit = 1000.00m,
                TotalDebit = 100.00m,
                Balance = 900.00m
            };

            var dto = _mapper.Map<ConsolidationDTO>(responseModel);

            // Assert
            dto.Should().NotBeNull();
            dto.Date.Should().Be(responseModel.Date);
            dto.TotalCredit.Should().Be(responseModel.TotalCredit);
            dto.TotalDebit.Should().Be(responseModel.TotalDebit);
            dto.Balance.Should().Be(responseModel.Balance);
        }
    }
}
