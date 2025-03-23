using AutoMapper;
using FluxoCaixa.Application.DTOs;
using FluxoCaixa.Domain.Entities;
using FluentAssertions;
using FluxoCaixa.Application.Mapping;

namespace FluxoCaixa.Tests.Application.Mapping
{
    public class MappingProfileTests
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IMapper _mapper;

        public MappingProfileTests()
        {
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
        public void Deve_Mapear_LaunchDTO_Para_Launch()
        {
            var launchDto = new LaunchDTO
            {
                Date = DateTime.Today,
                Type = Domain.Enum.LaunchType.Credit,
                Amount = 100.50m
            };

            var launch = _mapper.Map<Launch>(launchDto);

            launch.Date.Should().Be(launchDto.Date);
            launch.Type.Should().Be(launchDto.Type);
            launch.Amount.Should().Be(launchDto.Amount);
        }

        [Fact]
        public void Deve_Mapear_ConsolidationDTO_Para_Consolidation()
        {
            var consolidationDto = new ConsolidationDTO
            {
                Date = DateTime.Today,
                TotalCredit = 500.00m,
                TotalDebit = 200.00m,
                Balance = 300.00m
            };

            var consolidation = _mapper.Map<Consolidation>(consolidationDto);

            consolidation.Date.Should().Be(consolidationDto.Date);
            consolidation.TotalCredit.Should().Be(consolidationDto.TotalCredit);
            consolidation.TotalDebit.Should().Be(consolidationDto.TotalDebit);
            consolidation.Balance.Should().Be(consolidationDto.Balance);
        }
    }
}
