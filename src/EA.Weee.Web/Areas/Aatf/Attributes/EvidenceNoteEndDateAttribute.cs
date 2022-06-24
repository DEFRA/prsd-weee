namespace EA.Weee.Web.Areas.Aatf.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Filters;

    [AttributeUsage(AttributeTargets.Property)]
    public class EvidenceNoteEndDateAttribute : EvidenceDateValidationBase
    {
        public string CompareDatePropertyName { get; set; }

        public bool CheckComplianceYear { get; set; }

        public EvidenceNoteEndDateAttribute(string compareDatePropertyName, bool checkComplianceYear)
        {
            CompareDatePropertyName = compareDatePropertyName;
            CheckComplianceYear = checkComplianceYear;
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

            if (CheckComplianceYear)
            {
                if (thisDate.Year != currentDate.Year)
                {
                    //allowed range is first month of next compliance year
                    var allowedRangeStart = new DateTime(thisDate.Year + 1, 1, 1).Date;
                    var allowedRangeEnd = new DateTime(thisDate.Year + 1, 1, 31).Date;

                    if (!(currentDate.Date >= allowedRangeStart && currentDate.Date <= allowedRangeEnd) || thisDate.Year != currentDate.Year - 1)
                    {
                        return new ValidationResult("The end date must be within the current compliance year");
                    }
                }
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