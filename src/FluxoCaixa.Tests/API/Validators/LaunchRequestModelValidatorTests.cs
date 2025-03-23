using FluxoCaixa.API.Models;
using FluxoCaixa.API.Validators;
using FluxoCaixa.Domain.Enum;
using FluentValidation.TestHelper;

namespace FluxoCaixa.Tests.API.Validators
{
    public class LaunchRequestModelValidatorTests
    {
        private readonly LaunchRequestModelValidator _validator;

        public LaunchRequestModelValidatorTests()
        {
            _validator = new LaunchRequestModelValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Date_Is_Empty()
        {
            var model = new LaunchRequestModel
            {
                Date = default, 
                Type = LaunchType.Credit,
                Amount = 100.00m
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Date)
                .WithErrorMessage("A data é obrigatória.");
        }

        [Fact]
        public void Should_Have_Error_When_Date_Is_Before_One_Year_Ago()
        {
            var model = new LaunchRequestModel
            {
                Date = DateTime.Now.AddMonths(-13), // Data fora do limite de 1 ano
                Type = LaunchType.Credit,
                Amount = 50.00m
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Date)
                .WithErrorMessage("A data deve ser de 1 ano atrás para frente.");
        }

        [Fact]
        public void Should_Have_Error_When_Date_Is_In_Future()
        {
            var model = new LaunchRequestModel
            {
                Date = DateTime.Now.AddDays(1), 
                Type = LaunchType.Credit,
                Amount = 10.00m
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Date)
                .WithErrorMessage("A data deve ser no máximo igual a hoje.");
        }

        [Fact]
        public void Should_Have_Error_When_Type_Is_Invalid()
        {
            var model = new LaunchRequestModel
            {
                Date = DateTime.Now,
                Type = (LaunchType)999, 
                Amount = 20.00m
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Type)
                .WithErrorMessage("O tipo é inválido.");
        }

        [Fact]
        public void Should_Have_Error_When_Amount_Is_Less_Than_Zero()
        {
            var model = new LaunchRequestModel
            {
                Date = DateTime.Now,
                Type = LaunchType.Credit,
                Amount = -1.00m 
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Amount)
                .WithErrorMessage("A quantia deve ser maior que zero.");
        }

        [Fact]
        public void Should_Have_Error_When_Amount_Exceeds_Maximum()
        {
            var model = new LaunchRequestModel
            {
                Date = DateTime.Now,
                Type = LaunchType.Credit,
                Amount = 100000.00m 
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Amount)
                .WithErrorMessage("A quantia deve ser menor que 100.000.");
        }

        [Fact]
        public void Should_Have_Error_When_Amount_Has_Invalid_Precision()
        {
            var model = new LaunchRequestModel
            {
                Date = DateTime.Now,
                Type = LaunchType.Credit,
                Amount = 1000.123m 
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Amount)
                .WithErrorMessage("A quantia deve ter no máximo 10 dígitos e 2 casas decimais.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_Model_Is_Valid()
        {
            var model = new LaunchRequestModel
            {
                Date = DateTime.Now.AddMonths(-6), 
                Type = LaunchType.Credit,
                Amount = 99.99m 
            };

            var result = _validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
