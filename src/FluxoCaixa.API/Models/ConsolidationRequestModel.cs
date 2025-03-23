namespace FluxoCaixa.API.Models
{
    /// <summary>
    /// Classe de requisição da conciliação
    /// </summary>
    public class ConsolidationRequestModel
    {
        /// <summary>
        /// Data inicial
        /// </summary>
        public DateTime DateStart { get; set; }

        /// <summary>
        /// Data final
        /// </summary>
        public DateTime DateEnd { get; set; }
    }
}
