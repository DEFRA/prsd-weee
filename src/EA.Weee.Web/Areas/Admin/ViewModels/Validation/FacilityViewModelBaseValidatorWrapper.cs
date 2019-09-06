namespace EA.Weee.Web.Areas.Admin.ViewModels.Validation
{
    using EA.Weee.Api.Client;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using FluentValidation.Results;
    using System;
    using System.Threading.Tasks;

    public class FacilityViewModelBaseValidatorWrapper : IFacilityViewModelBaseValidatorWrapper
    {
        private readonly Func<IWeeeClient> apiClient;

        public FacilityViewModelBaseValidatorWrapper(Func<IWeeeClient> apiClient)
        {
            this.apiClient = apiClient;
        }

        public async Task<ValidationResult> Validate(string token, FacilityViewModelBase model)
        {
            var validator = new FacilityViewModelBaseValidator(token, apiClient, null);

            return await validator.ValidateAsync(model);
        }

        public async Task<ValidationResult> ValidateByYear(string token, FacilityViewModelBase model, int year)
        {
            var validator = new FacilityViewModelBaseValidator(token, apiClient, year);

            return await validator.ValidateAsync(model);
        }
    }
}