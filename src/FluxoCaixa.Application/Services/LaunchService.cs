using AutoMapper;
using FluxoCaixa.Application.DTOs;
using FluxoCaixa.Application.Interfaces;
using FluxoCaixa.Domain.Entities;
using FluxoCaixa.Domain.Exceptions;
using FluxoCaixa.Infra.Interfaces;
using Microsoft.Extensions.Logging;

namespace FluxoCaixa.Application.Services
{
    public class LaunchService(ILogger<LaunchService> logger, IMapper mapper, IRepository<Launch> launchRepository) : ILaunchService
    {
        private readonly ILogger<LaunchService> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly IRepository<Launch> _launchRepository = launchRepository;

        public async Task<IEnumerable<LaunchDTO>> GetByDateAsync(DateTime date)
        {
            _logger.LogInformation("Obtendo todos os lançamentos do dia {date}.", date);

            var launches = await _launchRepository.GetWhereAsync(r => r.Date == date);

            if (launches.Count() == 0)
                throw new NotFoundException("Lançamento não encontrado para a data.");

            var launchesDto = _mapper.Map<IEnumerable<LaunchDTO>>(launches);
            return launchesDto;
        }

        public async Task AddAsync(LaunchDTO launchDto)
        {
            _logger.LogInformation("Criando novo lançamento: {Tipo} - {Valor}", launchDto.Type, launchDto.Amount);
            var launch = _mapper.Map<Launch>(launchDto);

            var launchExists = await _launchRepository.GetByKeysAsync(launchDto.Date, launchDto.Type);
            if (launchExists != null)
                throw new DuplicateEntryException("Lançamento já cadastrado.");
            await _launchRepository.AddAsync(launch);
        }
    }
}
