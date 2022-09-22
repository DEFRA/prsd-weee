﻿namespace EA.Weee.Web.Areas.Aatf.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Core.Helpers;

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
            var currentDate = GetCurrentDateTime();

            if (value == null)
            {
                // let the required field validation deal with the entering of the date
                return ValidationResult.Success;
            }

            var thisDate = ((DateTime)value).Date;
            var otherDate = (DateTime?)validationContext.ObjectType.GetProperty(CompareDatePropertyName)?.GetValue(validationContext.ObjectInstance, null);

            if (CheckComplianceYear)
            {
                if (!WindowHelper.IsDateInComplianceYear(thisDate.Year, currentDate))
                {
                    return new ValidationResult("The end date must be within the current compliance year");
                }
            }

            return ValidateEndDate(otherDate, thisDate);
        }
    }
}