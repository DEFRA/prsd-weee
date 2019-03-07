namespace EA.Weee.Web.Areas.AatfReturn.ViewModels.Validation
{
    using Api.Client;
    using EA.Weee.Core.AatfReturn;
    using FluentValidation.Results;
    using System;
    using System.Threading.Tasks;

    public class NonObligatedValuesViewModelValidatorWrapper : INonObligatedValuesViewModelValidatorWrapper
    {
        public async virtual Task<ValidationResult> Validate(NonObligatedValuesViewModel instance, ReturnData returnData)
        {
            var validator = new NonObligatedValuesViewModelValidator(returnData);
            return await validator.ValidateAsync(instance);
        }
    }
}