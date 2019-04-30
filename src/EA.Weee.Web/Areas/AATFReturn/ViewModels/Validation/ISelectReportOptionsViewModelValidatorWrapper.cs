namespace EA.Weee.Web.Areas.AatfReturn.ViewModels.Validation
{
    using System.Threading.Tasks;
    using FluentValidation.Results;

    public interface ISelectReportOptionsViewModelValidatorWrapper
    {
        Task<ValidationResult> Validate(SelectReportOptionsViewModel instance);
    }
}