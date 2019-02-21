namespace EA.Weee.Web.Areas.AatfReturn.ViewModels.Validation
{
    using Api.Client;
    using Core.AatfReturn;
    using FluentValidation;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class NonObligatedValuesViewModelValidator : AbstractValidator<NonObligatedValuesViewModel>
    {
        private readonly Func<IWeeeClient> apiClient;

        public NonObligatedValuesViewModelValidator(Func<IWeeeClient> apiClient)
        {
            this.apiClient = apiClient;

            RuleFor(o => o)
                .MustAsync((o, cancellation) => ValidateValues(o.CategoryValues, o.Dcf))
                .WithMessage("my error");
        }

        private async Task<bool> ValidateValues(IList<NonObligatedCategoryValue> values, bool dcf)
        {
            using (var client = apiClient())
            {
                // retrieve the non obligated values
                // compare the totals and return result
                return await Task.FromResult(true);
            }
        }
    }
}