namespace EA.Weee.Web.Areas.AatfReturn.ViewModels.Validation
{
    using System;
    using System.Threading.Tasks;
    using Api.Client;
    using EA.Weee.Core.AatfReturn;
    using FluentValidation.Results;

    public interface INonObligatedValuesViewModelValidatorWrapper
    {
        Task<ValidationResult> Validate(NonObligatedValuesViewModel instance, ReturnData returnData);
    }
}