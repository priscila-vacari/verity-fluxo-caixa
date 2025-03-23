namespace FluxoCaixa.API.Models
{
    /// <summary>
    /// Classe de resposta da conciliação
    /// </summary>
    public class ConsolidationResponseModel
    {
        /// <summary>
        /// Data
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Total de crédito
        /// </summary>
        public decimal TotalCredit { get; set; }

        /// <summary>
        /// Total de débito
        /// </summary>
        public decimal TotalDebit { get; set; }

        /// <summary>
        /// Saldo
        /// </summary>
        public decimal Balance { get; set; }
    }
}
