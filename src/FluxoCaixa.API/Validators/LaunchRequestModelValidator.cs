using FluentValidation;
using FluxoCaixa.API.Models;
using FluxoCaixa.Domain.Enum;

namespace FluxoCaixa.API.Validators
{
    /// <summary>
    /// Classe de validação de lançamento
    /// </summary>
    public class LaunchRequestModelValidator : AbstractValidator<LaunchRequestModel>
    {
        /// <summary>
        /// Validador de lançamento
        /// </summary>
        public LaunchRequestModelValidator()
        {
            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("A data é obrigatória.")
                .GreaterThanOrEqualTo(DateTime.Now.AddMonths(-12)).WithMessage("A data deve ser de 1 ano atrás para frente.")
                .LessThan(DateTime.Now).WithMessage("A data deve ser no máximo igual a hoje.");

            RuleFor(x => x.Type)
                .Must(type => Enum.IsDefined(typeof(LaunchType), type))
                .WithMessage("O tipo é inválido.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("A quantia deve ser maior que zero.") 
                .LessThan(100000).WithMessage("A quantia deve ser menor que 100.000.")
                .PrecisionScale(10, 2, true).WithMessage("A quantia deve ter no máximo 10 dígitos e 2 casas decimais."); 
        }
    }
}
