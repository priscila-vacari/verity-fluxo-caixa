using FluxoCaixa.Application.DTOs;

namespace FluxoCaixa.Application.Interfaces
{
    public interface ILaunchService
    {
        Task<IEnumerable<LaunchDTO>> GetByDateAsync(DateTime date);
        Task AddAsync(LaunchDTO launchDto);
    }
}
