namespace EA.Weee.Web.Areas.Aatf.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Prsd.Core;
    using ViewModels;
    using Web.ViewModels.Shared;

    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredSubmitActionAttribute : RequiredAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = validationContext.ObjectInstance as IActionModel;

            Guard.ArgumentNotNull(() => model, model, "RequiredSubmitActionAttribute EditEvidenceNoteViewModel IsNull");

            if (model.Action.Equals(ActionEnum.Submit))
            {
                return base.IsValid(value, validationContext);
            }

            return ValidationResult.Success;
        }
    }
}