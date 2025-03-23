using FluxoCaixa.Application.DTOs;

namespace FluxoCaixa.Application.Interfaces
{
    public interface IConsolidationService
    {
        Task<ConsolidationDTO> GetByDateAsync(DateTime date);
        Task<IEnumerable<ConsolidationDTO>> GetByRangeDateAsync(DateTime dateStart, DateTime dateEnd);
        Task<ConsolidationDTO> GenerateDailyconsolidationAsync(DateTime date);
    }
}
