using FluxoCaixa.Domain.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace FluxoCaixa.Domain.Entities
{
    [Table("launches")]
    public class Launch
    {
        public DateTime Date { get; set; }
        public LaunchType Type { get; set; }
        public decimal Amount { get; set; }
    }
}
