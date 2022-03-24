namespace EA.Weee.Web.Areas.Aatf.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Prsd.Core;

    [AttributeUsage(AttributeTargets.Property)]
    public class EvidenceNoteEndDateAttribute : ValidationAttribute
    {
        public string CompareDatePropertyName { get; set; }

        public EvidenceNoteEndDateAttribute(string compareDatePropertyName)
        {
            CompareDatePropertyName = compareDatePropertyName;
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

            if (thisDate >= new DateTime(SystemTime.Now.Year + 1, 1, 1))
            {
                return new ValidationResult("The end date must be within the current compliance year");
            }

            if (otherDate.HasValue && !otherDate.Equals(DateTime.MinValue))
            {
                if (thisDate < otherDate)
                {
                    return new ValidationResult("Ensure the end date is after the start date");
                }
            }

            return ValidationResult.Success;
        }
    }
}