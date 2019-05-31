namespace EA.Weee.Web.Areas.Admin.ViewModels.Validation
{
    using System;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddAatf;
    using FluentValidation;
    using FluentValidation.Results;

    public class AatfViewModelValidator : AbstractValidator<AatfViewModelBase>
    {
        public AatfViewModelValidator()
        {
            RuleFor(o => o.ApprovalDate)
                .Custom((o, context) =>
                {
                    var instance = context.InstanceToValidate as AatfViewModelBase;

                    if (instance?.ApprovalDate != null)
                    {
                        DateTime input = (DateTime)instance?.ApprovalDate;

                        var startDate = new DateTime(instance.ComplianceYear, 1, 1);
                        var endDate = new DateTime(instance.ComplianceYear, 12, 31);

                        if (input <= startDate || input >= endDate)
                        {
                            context.AddFailure(new ValidationFailure($"ApprovalDate", $"Approval date must be between {startDate.ToString("dd/MM/yyyy")} and {endDate.ToString("dd/MM/yyyy")}"));
                        }
                    }
                });
        }
    }
}