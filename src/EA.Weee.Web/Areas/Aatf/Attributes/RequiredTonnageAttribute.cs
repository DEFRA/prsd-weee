namespace EA.Weee.Web.Areas.Aatf.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Prsd.Core;
    using ViewModels;
    using Web.ViewModels.Shared;

    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredTonnageAttribute : RequiredTonnageBaseAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = validationContext.ObjectInstance as IActionModel;

            Guard.ArgumentNotNull(() => model, model, "RequiredTonnageAttribute Model is null");
            Guard.ArgumentNotNull(() => value, value, "RequiredTonnageAttribute Tonnage Values are null");

            if (model.Action.Equals(ActionEnum.Submit))
            {
                return ValidateTonnage(value);
            }

            return ValidationResult.Success;
        }
    }
}