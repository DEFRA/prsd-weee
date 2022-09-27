namespace EA.Weee.Web.Areas.Aatf.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [AttributeUsage(AttributeTargets.Property)]
    public class EvidenceNoteFilterStartDateAttribute : EvidenceDateValidationBase
    {
        public EvidenceNoteFilterStartDateAttribute(string compareDatePropertyName) : base(compareDatePropertyName)
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var currentDate = GetCurrentDateTime();

            if (value == null)
            {
                // let the required field validation deal with the entering of the date
                return ValidationResult.Success;
            }

            var thisDate = ((DateTime)value).Date;
            var otherDate = (DateTime?)validationContext.ObjectType.GetProperty(CompareDatePropertyName)?.GetValue(validationContext.ObjectInstance, null);

            return ValidateStartDate(thisDate, otherDate, currentDate);
        }
    }
}