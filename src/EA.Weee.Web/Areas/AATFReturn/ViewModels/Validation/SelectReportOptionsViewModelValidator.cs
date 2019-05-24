namespace EA.Weee.Web.Areas.AatfReturn.ViewModels.Validation
{
    using System.Linq;
    using FluentValidation;
    using FluentValidation.Results;

    public class SelectReportOptionsViewModelValidator : AbstractValidator<SelectReportOptionsViewModel>
    {
        public SelectReportOptionsViewModelValidator()
        {
            RuleFor(o => o.DcfSelectedValue)
                .Custom((o, context) =>
            {
                var instance = context.InstanceToValidate as SelectReportOptionsViewModel;

                if (instance?.SelectedOptions != null)
                {
                    var dcfQuestion = instance.ReportOnQuestions.FirstOrDefault(d => d.ParentId != default(int));
                    var isParentSelected = dcfQuestion != null && instance.SelectedOptions.Contains(dcfQuestion.ParentId ?? default(int));

                    if (isParentSelected && !instance.DcfPossibleValues.Contains(instance.DcfSelectedValue))
                    {
                        dcfQuestion.HasError = true;
                        context.AddFailure(new ValidationFailure($"Option-{dcfQuestion.Id-1}", $"You must tell us whether any of the non-obligated WEEE was retained by a DCF"));
                    }
                }
            });
        }
    }
}