namespace FluxoCaixa.Application.DTOs
{
    public class ConsolidationDTO
    {
        public DateTime Date { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal Balance { get; set; }
    }
}
