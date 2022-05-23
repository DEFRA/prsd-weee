namespace EA.Weee.Web.Areas.Aatf.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Filters;

    [AttributeUsage(AttributeTargets.Property)]
    public class EvidenceNoteStartDateAttribute : EvidenceDateValidationBase
    {
        public string CompareDatePropertyName { get; set; }

        public EvidenceNoteStartDateAttribute(string compareDatePropertyName)
        {
            CompareDatePropertyName = compareDatePropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var currentDate = AsyncHelpers.RunSync(async () => await Cache.FetchCurrentDate());

            if (value == null)
            {
                // let the required field validation deal with the entering of the date
                return ValidationResult.Success;
            }

            var thisDate = ((DateTime)value).Date;
            var otherDate = (DateTime?)validationContext.ObjectType.GetProperty(CompareDatePropertyName)?.GetValue(validationContext.ObjectInstance, null);

            if (thisDate > DateTime.UtcNow.Date)
            {
                return new ValidationResult("The start date cannot be in the future. Select today's date or earlier.");
            }

            if (thisDate < new DateTime(currentDate.Year, 1, 1))
            {
                return new ValidationResult("The start date must be within the current compliance year");
            }

            if (otherDate.HasValue && !otherDate.Equals(DateTime.MinValue))
            {
                if (thisDate > otherDate.Value.Date)
                {
                    return new ValidationResult("Ensure the start date is before the end date");
                }
            }

            return ValidationResult.Success;
        }
    }
}