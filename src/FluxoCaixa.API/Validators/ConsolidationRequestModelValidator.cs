using FluentValidation;
using FluxoCaixa.API.Models;

namespace FluxoCaixa.API.Validators
{
    /// <summary>
    /// Classe de validação de conciliação
    /// </summary>
    public class ConsolidationRequestModelValidator : AbstractValidator<ConsolidationRequestModel>
    {
        /// <summary>
        /// Validador de conciliação
        /// </summary>
        public ConsolidationRequestModelValidator()
        {
            RuleFor(x => x.DateStart)
                .LessThanOrEqualTo(x => x.DateEnd)
                .WithMessage("A data inicial não pode ser maior que a data final.");

            RuleFor(x => x.DateStart)
                .GreaterThan(DateTime.Now.AddYears(-1))
                .WithMessage("A data inicial deve ser maior que 1 ano atrás.");

            RuleFor(x => x.DateEnd)
                .GreaterThan(DateTime.Now.AddYears(-1))
                .WithMessage("A data final deve ser maior que 1 ano atrás.");

            RuleFor(x => x.DateStart)
                .LessThanOrEqualTo(DateTime.Now)
                .WithMessage("A data inicial não pode ser maior que a data de hoje.");

            RuleFor(x => x.DateEnd)
                .LessThanOrEqualTo(DateTime.Now)
                .WithMessage("A data final não pode ser maior que a data de hoje.");
        }
    }
}
