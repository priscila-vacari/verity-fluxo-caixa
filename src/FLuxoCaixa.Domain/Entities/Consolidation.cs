using System.ComponentModel.DataAnnotations.Schema;

namespace FluxoCaixa.Domain.Entities
{
    [Table("consolidations")]
    public class Consolidation
    {
        public DateTime Date { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal Balance { get; set; }
    }
}
