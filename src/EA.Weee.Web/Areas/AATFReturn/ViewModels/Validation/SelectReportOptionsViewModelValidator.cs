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
                if (instance != null && instance.SelectedOptions != null)
                {
                    var dcfQuestion = instance.ReportOnQuestions.Where(d => d.ParentId != default(int)).FirstOrDefault();
                    bool isParentSelected = instance.SelectedOptions.Contains(dcfQuestion.ParentId ?? default(int));
                    if (isParentSelected && !instance.DcfPossibleValues.Contains(instance.DcfSelectedValue))
                    {
                        context.AddFailure(new ValidationFailure($"DcfSelectedValue", $"You must tell us whether any of the non-obligated WEEE was retained by a DCF"));
                    }
                }
            });
        }
    }
}