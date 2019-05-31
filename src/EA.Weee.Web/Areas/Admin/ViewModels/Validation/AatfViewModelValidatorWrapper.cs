namespace EA.Weee.Web.Areas.Admin.ViewModels.Validation
{
    using System.Threading.Tasks;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddAatf;
    using FluentValidation.Results;

    public class AatfViewModelValidatorWrapper : IAatfViewModelValidatorWrapper
    {
        public async virtual Task<ValidationResult> Validate(AatfViewModelBase instance)
        {
            var validator = new AatfViewModelValidator();
            return await validator.ValidateAsync(instance);
        }
    }
}