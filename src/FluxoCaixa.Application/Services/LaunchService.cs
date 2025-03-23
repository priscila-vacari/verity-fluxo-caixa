using AutoMapper;
using FluxoCaixa.Application.DTOs;
using FluxoCaixa.Application.Interfaces;
using FluxoCaixa.Domain.Entities;
using FluxoCaixa.Domain.Exceptions;
using FluxoCaixa.Infra.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FluxoCaixa.Application.Services
{
    public class LaunchService(ILogger<LaunchService> logger, IMapper mapper, IServiceProvider serviceProvider) : ILaunchService
    {
        private readonly ILogger<LaunchService> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public async Task<IEnumerable<LaunchDTO>> GetByDateAsync(DateTime date)
        {
            _logger.LogInformation("Obtendo todos os lançamentos do dia {date}.", date);

            using var scope = _serviceProvider.CreateScope();
            var _launchRepository = scope.ServiceProvider.GetRequiredService<IRepository<Launch>>();
            var launches = await _launchRepository.GetWhereAsync(r => r.Date == date);

            var launchesDto = _mapper.Map<IEnumerable<LaunchDTO>>(launches);
            return launchesDto;
        }

        public async Task AddAsync(LaunchDTO launchDto)
        {
            _logger.LogInformation("Criando novo lançamento: {Tipo} - {Valor}", launchDto.Type, launchDto.Amount);
            var launch = _mapper.Map<Launch>(launchDto);

            using var scope = _serviceProvider.CreateScope();
            var _launchRepository = scope.ServiceProvider.GetRequiredService<IRepository<Launch>>();

            var launchExists = await _launchRepository.GetByKeysAsync(launchDto.Date, launchDto.Type);
            if (launchExists != null)
                throw new DuplicateEntryException("Lançamento já cadastrado.");
            await _launchRepository.AddAsync(launch);
        }
    }
}
