namespace EA.Weee.Web.Areas.AatfReturn.ViewModels.Validation
{
    using Core.AatfReturn;
    using FluentValidation;
    using FluentValidation.Results;
    using System.Linq;

    public class NonObligatedValuesViewModelValidator : AbstractValidator<NonObligatedValuesViewModel>
    {
        private ReturnData returnData;

        public NonObligatedValuesViewModelValidator(ReturnData returnData)
        {
            this.returnData = returnData;

            RuleForEach(o => o.CategoryValues)
                .Custom((o, context) =>
                {
                    var instance = context.InstanceToValidate as NonObligatedValuesViewModel;

                    if (instance != null && instance.Dcf)
                    {
                        var returnTonnage = returnData.NonObligatedData.Where(r => r.CategoryId == o.CategoryId && r.Dcf == false).Select(r => r.Tonnage).FirstOrDefault();
                        var value = 0.000m;

                        if (o.Tonnage != null)
                        {
                            decimal.TryParse(o.Tonnage, out value);

                            if (value > returnTonnage)
                            {
                                var categoryFocus = o.CategoryId - 1;
                                context.AddFailure(new ValidationFailure($"TotalCategoryValues[{categoryFocus}].Tonnage",
                                    $"Category {o.CategoryId} tonnage must be less than or equal to {returnTonnage}"));
                            }
                        }
                    }
                    else if (instance != null && !instance.Dcf)
                    {
                        var returnTonnage = returnData.NonObligatedData.Where(r => r.CategoryId == o.CategoryId && r.Dcf == true).Select(r => r.Tonnage).FirstOrDefault();
                        var value = 0.000m;

                        if (o.Tonnage != null)
                        {
                            decimal.TryParse(o.Tonnage, out value);

                            if (value < returnTonnage)
                            {
                                var categoryFocus = o.CategoryId - 1;
                                context.AddFailure(new ValidationFailure($"TotalCategoryValues[{categoryFocus}].Tonnage",
                                    $"Category {o.CategoryId} tonnage must be greater than or equal to {returnTonnage}"));
                            }
                        }
                        else if (o.Tonnage == null && returnTonnage != null)
                        {
                            var categoryFocus = o.CategoryId - 1;
                            context.AddFailure(new ValidationFailure($"TotalCategoryValues[{categoryFocus}].Tonnage",
                                $"Category {o.CategoryId} tonnage must be greater than or equal to {returnTonnage}"));
                        }
                    }
                });
        }
    }
}