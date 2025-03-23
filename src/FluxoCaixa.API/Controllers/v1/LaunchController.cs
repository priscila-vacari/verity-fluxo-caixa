using Asp.Versioning;
using AutoMapper;
using FluxoCaixa.API.Models;
using FluxoCaixa.Application.DTOs;
using FluxoCaixa.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FluxoCaixa.API.Controllers.v1
{
    /// <summary>
    /// Controller responsável pelos lançamentos
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="mapper"></param>
    /// <param name="serviceFactory"></param>
    [ApiVersion("1.0")]
    public class LaunchController(ILogger<LaunchController> logger, IMapper mapper, IServiceFactory serviceFactory) : BaseController(logger, mapper)
    {
        private readonly IServiceFactory _serviceFactory = serviceFactory;

        /// <summary>
        /// Busca os lançamentos de uma data
        /// </summary>
        /// <param name="date"></param>
        /// <returns>Retorna todos os lançamentos encontrados da data específica</returns>
        [HttpGet("{date}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetLaunchByDate(DateTime date)
        {
            _logger.LogInformation("Obter todos os lançamentos do dia {date}", date);

            var launchService = _serviceFactory.CreateLaunchService();
            var launches = await launchService.GetByDateAsync(date);

            var launchResponseModel = _mapper.Map<IEnumerable<LaunchResponseModel>>(launches);
            return Ok(launchResponseModel);
        }

        /// <summary>
        /// Adiciona um novo lançamento
        /// </summary>
        /// <param name="launchModel"></param>
        /// <returns>Retorna o lançamento criado</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddLaunch([FromBody] LaunchRequestModel launchModel)
        {
            _logger.LogInformation("Criar novo lançamento para data {date}", launchModel.Date);

            var launchDto = _mapper.Map<LaunchDTO>(launchModel);

            var launchService = _serviceFactory.CreateLaunchService();
            await launchService.AddAsync(launchDto);

            var launchResponseModel = _mapper.Map<LaunchResponseModel>(launchDto);
            return CreatedAtAction(nameof(AddLaunch), launchResponseModel);
        }
    }
}
