namespace EA.Weee.Web.Areas.AatfReturn.ViewModels.Validation
{
    using Api.Client;
    using Core.AatfReturn;
    using EA.Weee.Requests.AatfReturn;
    using FluentValidation;
    using Security;
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
                .MustAsync((o, cancellation) => ValidateValues(o.CategoryValues, o.Dcf, o.ReturnId))
                .WithMessage("my error");

            //RuleForEach(o => o.CategoryValues).MustAsync((o, cancellation) => ValidateValues(o.CategoryId, o.Dcf, o.Tonnage)).WithMessage("Error Test");
        }

        private async Task<bool> ValidateValues(IList<NonObligatedCategoryValue> values, bool dcf, Guid returnId)
        {
            using (var client = apiClient())
            {
                if (!dcf)
                {
                }
                else
                {
                    /*
                    var result = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId));
                    for (var i = 0; i < values.Count; i++)
                    {
                        var value = decimal.Parse(values[i].Tonnage);
                        if (value > result.NonObligatedData[i].Tonnage)
                        {
                            var yes = "yes";
                        }
                    }*/
                }
                // retrieve the non obligated values
                // compare the totals and return result
                return await Task.FromResult(true);
            }
        }
    }
}