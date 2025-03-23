using FluxoCaixa.Domain.Enum;

namespace FluxoCaixa.API.Models
{
    /// <summary>
    /// Classe de requisição do lançamento
    /// </summary>
    public class LaunchRequestModel
    {
        /// <summary>
        /// Data
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Tipo (crédito ou débito)
        /// </summary>
        public LaunchType Type { get; set; }

        /// <summary>
        /// Valor
        /// </summary>
        public decimal Amount { get; set; }
    }
}
