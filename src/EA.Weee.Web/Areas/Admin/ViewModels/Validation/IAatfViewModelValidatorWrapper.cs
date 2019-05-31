namespace EA.Weee.Web.Areas.Admin.ViewModels.Validation
{
    using System.Threading.Tasks;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddAatf;
    using FluentValidation.Results;

    public interface IAatfViewModelValidatorWrapper
    {
        Task<ValidationResult> Validate(AatfViewModelBase instance);
    }
}