namespace EA.Weee.Web.Areas.Admin.ViewModels.Validation
{
    using System.Threading.Tasks;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using FluentValidation.Results;

    public interface IFacilityViewModelBaseValidatorWrapper
    {
        Task<ValidationResult> Validate(string token, FacilityViewModelBase model);

        Task<ValidationResult> ValidateByYear(string token, FacilityViewModelBase model, int year);
    }
}