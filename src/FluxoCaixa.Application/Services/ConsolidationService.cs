﻿using AutoMapper;
using FluxoCaixa.Application.DTOs;
using FluxoCaixa.Application.Interfaces;
using FluxoCaixa.Domain.Entities;
using FluxoCaixa.Domain.Enum;
using FluxoCaixa.Domain.Exceptions;
using FluxoCaixa.Infra.Interfaces;
using Microsoft.Extensions.Logging;

namespace FluxoCaixa.Application.Services
{
    public class ConsolidationService(ILogger<ConsolidationService> logger, IMapper mapper, ILaunchService launchService, IRepository<Consolidation> consolidationRepository) : IConsolidationService
    {
        private readonly ILogger<ConsolidationService> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly ILaunchService _launchService = launchService;
        private readonly IRepository<Consolidation> _consolidationRepository = consolidationRepository;

        public async Task<ConsolidationDTO> GetByDateAsync(DateTime date)
        {
            _logger.LogInformation("Obtendo consolidado da data: {date}", date);

            var consolidation = await _consolidationRepository.GetByKeysAsync(date) ?? throw new NotFoundException("Consolidação não encontrada para esta data.");
            
            return _mapper.Map<ConsolidationDTO>(consolidation);
        }

        public async Task<IEnumerable<ConsolidationDTO>> GetByRangeDateAsync(DateTime dateStart, DateTime dateEnd)
        {
            _logger.LogInformation("Obtendo consolidado do período: {dateStart} a {dateEnd}", dateStart, dateEnd);

            var consolidations = await _consolidationRepository.GetWhereAsync(d => d.Date >= dateStart && d.Date <= dateEnd);

            if (consolidations.Count() == 0)
                throw new NotFoundException("Consolidação não encontrada para o período.");

            return _mapper.Map<IEnumerable<ConsolidationDTO>>(consolidations);
        }

        public async Task<ConsolidationDTO> GenerateDailyconsolidationAsync(DateTime date)
        {
            _logger.LogInformation("Gerando consolidação para a data: {date}", date);

            var launches = await _launchService.GetByDateAsync(date);

            var consolidation = new Consolidation
            {
                Date = date,
                TotalCredit = launches.Where(l => l.Type == LaunchType.Credit).Sum(l => l.Amount),
                TotalDebit = launches.Where(l => l.Type == LaunchType.Debit).Sum(l => l.Amount)
            };
            consolidation.Balance = consolidation.TotalCredit - consolidation.TotalDebit;

            await _consolidationRepository.DeleteAsync(date);
            await _consolidationRepository.AddAsync(consolidation);

            return _mapper.Map<ConsolidationDTO>(consolidation);
        }
    }
}
