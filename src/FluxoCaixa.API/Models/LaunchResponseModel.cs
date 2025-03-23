using FluxoCaixa.Domain.Enum;

namespace FluxoCaixa.API.Models
{
    /// <summary>
    /// Classe de resposta do lançamento
    /// </summary>
    public class LaunchResponseModel
    {
        /// <summary>
        /// Data
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Tipo
        /// </summary>
        public LaunchType Type { get; set; }

        /// <summary>
        /// Valor
        /// </summary>
        public decimal Amount { get; set; }
    }
}
