namespace EA.Weee.Web.Areas.AatfReturn.ViewModels.Validation
{
    using Api.Client;
    using FluentValidation.Results;
    using System;

    public class NonObligatedValuesViewModelValidatorWrapper : INonObligatedValuesViewModelValidatorWrapper
    {
        // MAKE THIS ASYNC
        public ValidationResult Validate(NonObligatedValuesViewModel instance, string token, Func<IWeeeClient> apiClient)
        {
            var validator = new NonObligatedValuesViewModelValidator(apiClient, token);
            return validator.Validate(instance);
        }
    }
}