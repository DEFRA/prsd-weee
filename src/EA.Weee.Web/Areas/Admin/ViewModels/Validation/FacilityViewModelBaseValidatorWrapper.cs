namespace EA.Weee.Web.Areas.Admin.ViewModels.Validation
{
    using EA.Weee.Api.Client;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using FluentValidation.Results;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;

    public class FacilityViewModelBaseValidatorWrapper : IFacilityViewModelBaseValidatorWrapper
    {
        private readonly Func<IWeeeClient> apiClient;

        public FacilityViewModelBaseValidatorWrapper(Func<IWeeeClient> apiClient)
        {
            this.apiClient = apiClient;
        }

        public async Task<ValidationResult> Validate(string token, FacilityViewModelBase model)
        {
            var validator = new FacilityViewModelBaseValidator(token, apiClient, model, null);

            return await validator.ValidateAsync(model);
        }

        public async Task<ValidationResult> ValidateByYear(string token, FacilityViewModelBase model, int year)
        {
            var validator = new FacilityViewModelBaseValidator(token, apiClient, model, year);

            return await validator.ValidateAsync(model);
        }
    }
}