namespace EA.Weee.Web.Areas.AatfReturn.ViewModels.Validation
{
    using Api.Client;
    using Core.AatfReturn;
    using FluentValidation;
    using FluentValidation.Results;
    using FluentValidation.Validators;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Weee.Requests.AatfReturn;

    public class NonObligatedValuesViewModelValidator : AbstractValidator<NonObligatedValuesViewModel>
    {
        private ReturnData returnData;

        public NonObligatedValuesViewModelValidator(ReturnData returnData)
        {
            this.returnData = returnData;

            RuleForEach(o => o.CategoryValues)
                .Custom((o, context) => 
                {
                    List<NonObligatedData> orderedReturnWeee = returnData.NonObligatedData.OrderBy(r => r.CategoryId).ToList();
                    var returnTonnage = returnData.NonObligatedData.Where(r => r.CategoryId == o.CategoryId).Select(r => r.Tonnage).FirstOrDefault();
                    var value = 0.000m;

                    if (o.Tonnage != null)
                    {
                        value = decimal.Parse(o.Tonnage);
                    }

                    if (value > returnTonnage)
                    {
                        context.AddFailure(new ValidationFailure($"o.Tonnage",
                            $"Category {o.CategoryId} tonnage must be less or equal to {returnTonnage}"));
                    }
                });
        }
    }
}