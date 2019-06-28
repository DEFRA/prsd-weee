namespace EA.Weee.Web.Areas.AatfReturn.ViewModels.Validation
{
    using System;
    using System.Linq;
    using Core.AatfReturn;
    using FluentValidation;
    using FluentValidation.Results;
    using Prsd.Core.Domain;

    public class SelectReportOptionsViewModelValidator : AbstractValidator<SelectReportOptionsViewModel>
    {
        public SelectReportOptionsViewModelValidator()
        {
            RuleFor(o => o.DcfSelectedValue)
                .Custom((o, context) =>
            {
                var instance = context.InstanceToValidate as SelectReportOptionsViewModel;

                if (instance == null)
                {
                    throw new ArgumentNullException("SelectReportOptionsViewModel");
                }

                if (!instance.HasSelectedOptions)
                {
                    context.AddFailure(new ValidationFailure($"hasSelectedOptions", $"You must select at least one reporting option, unless you have no data to report"));
                }
                else
                {
                    if (instance.DcfQuestionSelected && !instance.DcfPossibleValues.Contains(instance.DcfSelectedValue))
                    {
                        instance.DcfQuestion.HasError = true;
                        context.AddFailure(new ValidationFailure($"Option-{instance.DcfQuestion.Id - 1}", $"You must tell us whether any of the non-obligated WEEE was retained by a DCF"));
                    }
                }
            });
        }
    }
}