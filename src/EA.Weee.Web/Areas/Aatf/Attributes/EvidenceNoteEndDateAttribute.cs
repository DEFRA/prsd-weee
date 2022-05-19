namespace EA.Weee.Web.Areas.Aatf.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Filters;
    using Prsd.Core;
    using Weee.Requests.Shared;

    [AttributeUsage(AttributeTargets.Property)]
    public class EvidenceNoteEndDateAttribute : EvidenceDateValidationBase
    {
        public string CompareDatePropertyName { get; set; }

        public EvidenceNoteEndDateAttribute(string compareDatePropertyName)
        {
            CompareDatePropertyName = compareDatePropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            using (var client = Client())
            {
                var userToken = HttpContextService.GetAccessToken();
                var currentDate =
                    AsyncHelpers.RunSync(async () => await client.SendAsync(userToken, new GetApiUtcDate()));

                if (value == null)
                {
                    // let the required field validation deal with the entering of the date
                    return ValidationResult.Success;
                }

                var thisDate = ((DateTime)value).Date;
                var otherDate = (DateTime?)validationContext.ObjectType.GetProperty(CompareDatePropertyName)?.GetValue(validationContext.ObjectInstance, null);

                if (thisDate >= new DateTime(currentDate.Year + 1, 1, 1))
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
}