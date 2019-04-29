namespace EA.Weee.Web.Areas.AatfReturn.ViewModels.Validation
{
    using System.Threading.Tasks;
    using FluentValidation;
    using FluentValidation.Results;

    public class SelectReportOptionsViewModelValidatorWrapper : ISelectReportOptionsViewModelValidatorWrapper
    {
        public async virtual Task<ValidationResult> Validate(SelectReportOptionsViewModel instance)
        {
            var validator = new SelectReportOptionsViewModelValidator();
            return await validator.ValidateAsync(instance);
        }
    }
}