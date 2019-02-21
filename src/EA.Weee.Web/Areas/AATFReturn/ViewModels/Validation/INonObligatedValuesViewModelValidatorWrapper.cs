namespace EA.Weee.Web.Areas.AatfReturn.ViewModels.Validation
{
    using System;
    using Api.Client;
    using FluentValidation.Results;

    public interface INonObligatedValuesViewModelValidatorWrapper
    {
        ValidationResult Validate(NonObligatedValuesViewModel instance, string token, Func<IWeeeClient> apiClient);
    }
}