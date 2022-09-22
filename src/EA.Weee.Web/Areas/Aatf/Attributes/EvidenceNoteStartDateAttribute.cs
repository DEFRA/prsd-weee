﻿namespace EA.Weee.Web.Areas.Aatf.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Core.Helpers;
    using EA.Prsd.Core;

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
            var currentDate = GetCurrentDateTime();

            if (value == null)
            {
                // let the required field validation deal with the entering of the date
                return ValidationResult.Success;
            }

            var thisDate = ((DateTime)value).Date;
            var otherDate = (DateTime?)validationContext.ObjectType.GetProperty(CompareDatePropertyName)?.GetValue(validationContext.ObjectInstance, null);

            if (!WindowHelper.IsDateInComplianceYear(thisDate.Year, currentDate))
            {
                return new ValidationResult("The start date must be within the current compliance year");
            }
            
            return ValidateStartDate(thisDate, otherDate, currentDate);
        }
    }
}