namespace EA.Weee.Web.Areas.Aatf.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Prsd.Core;
    using ViewModels;

    [AttributeUsage(AttributeTargets.Class)]
    public class TestAttribute : RequiredAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = validationContext.ObjectInstance as EvidenceNoteViewModel;

            Guard.ArgumentNotNull(() => model, model, "RequiredSubmitActionAttribute EvidenceViewModel IsNull");

            return new ValidationResult("error");
        }
    }
}