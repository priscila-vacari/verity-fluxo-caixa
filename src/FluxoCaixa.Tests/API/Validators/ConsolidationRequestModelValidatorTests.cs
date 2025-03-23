using FluentValidation.TestHelper;
using FluxoCaixa.API.Models;
using FluxoCaixa.API.Validators;

namespace FluxoCaixa.Tests.API.Validators
{
    public class ConsolidationRequestModelValidatorTests
    {
        private readonly ConsolidationRequestModelValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_DateStart_Is_Greater_Than_DateEnd()
        {
            var model = new ConsolidationRequestModel
            {
                DateStart = DateTime.Now,
                DateEnd = DateTime.Now.AddDays(-1)
            };

            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.DateStart);
        }

        [Fact]
        public void Should_Have_Error_When_DateStart_Is_Less_Than_One_Year_Ago()
        {
            var model = new ConsolidationRequestModel
            {
                DateStart = DateTime.Now.AddYears(-2),
                DateEnd = DateTime.Now
            };

            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.DateStart);
        }

        [Fact]
        public void Should_Have_No_Error_For_Valid_Dates()
        {
            var model = new ConsolidationRequestModel
            {
                DateStart = DateTime.Now.AddDays(-10),
                DateEnd = DateTime.Now.AddDays(-5)
            };

            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}