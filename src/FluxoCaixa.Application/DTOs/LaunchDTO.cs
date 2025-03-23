using FluxoCaixa.Domain.Enum;

namespace FluxoCaixa.Application.DTOs
{
    public class LaunchDTO
    {
        public DateTime Date { get; set; }
        public LaunchType Type { get; set; }
        public decimal Amount { get; set; }
    }
}
