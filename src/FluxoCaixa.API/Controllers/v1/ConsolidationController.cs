using Asp.Versioning;
using AutoMapper;
using FluxoCaixa.API.Models;
using FluxoCaixa.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FluxoCaixa.API.Controllers.v1
{
    /// <summary>
    /// Controller responsável pelas conciliações
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="mapper"></param>
    /// <param name="serviceFactory"></param>
    [ApiVersion("1.0")]
    public class ConsolidationController(ILogger<LaunchController> logger, IMapper mapper, IServiceFactory serviceFactory) : BaseController(logger, mapper)
    {
        private readonly IServiceFactory _serviceFactory = serviceFactory;

        /// <summary>
        /// Busca a conciliação de uma data
        /// </summary>
        /// <param name="date"></param>
        /// <returns>Retorna a conciliação encontrada da data específica</returns>
        [HttpGet("{date}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetConsolidationByDate(DateTime date)
        {
            _logger.LogInformation("Obter a consolidação do dia {date}", date);

            var consolidationService = _serviceFactory.CreateConsolidationService();
            var consolidation = await consolidationService.GetByDateAsync(date);

            var consolidationModel = _mapper.Map<ConsolidationResponseModel>(consolidation);
            return Ok(consolidationModel);
        }

        /// <summary>
        /// Busca todas as conciliações de um range de datas
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Retorna as conciliações encontradas do range de datas</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetConsolidationByRangeDate([FromBody] ConsolidationRequestModel request)
        {
            _logger.LogInformation("Obter a consolidação do período {dateStart} a {dateEnd}", request.DateStart, request.DateEnd);

            var consolidationService = _serviceFactory.CreateConsolidationService();
            var consolidations = await consolidationService.GetByRangeDateAsync(request.DateStart, request.DateEnd);

            var consolidationsModel = _mapper.Map<IEnumerable<ConsolidationResponseModel>>(consolidations);
            return Ok(consolidationsModel);
        }

        /// <summary>
        /// Gera uma nova conciliação a partir de uma data
        /// </summary>
        /// <param name="date"></param>
        /// <returns>Retorna a conciliação gerada</returns>
        [HttpPost("{date}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GenerateDailyconsolidationAsync(DateTime date)
        {
            _logger.LogInformation("Gerar a consolidação para data {date}", date);

            var consolidationService = _serviceFactory.CreateConsolidationService();
            var consolidation = await consolidationService.GenerateDailyconsolidationAsync(date);

            var consolidationModel = _mapper.Map<ConsolidationResponseModel>(consolidation);
            return new CreatedResult(string.Empty, consolidationModel);
        }
    }
}
