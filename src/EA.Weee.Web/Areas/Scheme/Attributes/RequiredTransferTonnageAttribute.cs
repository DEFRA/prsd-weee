namespace EA.Weee.Web.Areas.Scheme.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Aatf.Attributes;
    using EA.Prsd.Core;
    using EA.Weee.Web.Areas.Aatf.ViewModels;

    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredTransferTonnageAttribute : RequiredTonnageBaseAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Guard.ArgumentNotNull(() => value, value, "RequiredTransferTonnageAttribute Tonnage Values are null");

            return ValidateTonnage(value);
        }
    }
}