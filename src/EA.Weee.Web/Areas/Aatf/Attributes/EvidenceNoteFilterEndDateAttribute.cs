namespace EA.Weee.Web.Areas.Aatf.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [AttributeUsage(AttributeTargets.Property)]
    public class EvidenceNoteFilterEndDateAttribute : EvidenceDateValidationBase
    {
        public EvidenceNoteFilterEndDateAttribute(string compareDatePropertyName) : base(compareDatePropertyName)
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                // let the required field validation deal with the entering of the date
                return ValidationResult.Success;
            }

            var thisDate = ((DateTime)value).Date;
            var otherDate = (DateTime?)validationContext.ObjectType.GetProperty(CompareDatePropertyName)?.GetValue(validationContext.ObjectInstance, null);

            return ValidateEndDate(otherDate, thisDate);
        }
    }
}