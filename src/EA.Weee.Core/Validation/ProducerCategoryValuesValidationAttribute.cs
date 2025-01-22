﻿namespace EA.Weee.Core.Validation
{
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Helpers;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ProducerCategoryValuesValidationAttribute : ValidationAttribute
    {
        public override bool RequiresValidationContext => true;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var categoryValues = value as IList<ProducerSubmissionCategoryValue>;
            if (categoryValues == null)
            {
                return new ValidationResult("Category values cannot be null", new[] { validationContext.MemberName });
            }

            // Check for invalid inputs before using the calculator
            foreach (var cv in categoryValues)
            {
                if (!string.IsNullOrEmpty(cv.HouseHold) && !decimal.TryParse(cv.HouseHold, out _))
                {
                    return new ValidationResult("Invalid household total", new[] { validationContext.MemberName });
                }
                if (!string.IsNullOrEmpty(cv.NonHouseHold) && !decimal.TryParse(cv.NonHouseHold, out _))
                {
                    return new ValidationResult("Invalid non-household total", new[] { validationContext.MemberName });
                }
            }

            var calculator = new CategoryValueTotalCalculator();
            var totalHouseHold = calculator.Total(categoryValues.Select(t => t.HouseHold).Where(v => v != null).ToList());
            var totalNonHouseHold = calculator.Total(categoryValues.Select(t => t.NonHouseHold).Where(v => v != null).ToList());

            if (!decimal.TryParse(totalHouseHold, out var householdDecimal))
            {
                return new ValidationResult("Invalid household total", new[] { validationContext.MemberName });
            }
            if (!decimal.TryParse(totalNonHouseHold, out var nonHouseholdDecimal))
            {
                return new ValidationResult("Invalid non-household total", new[] { validationContext.MemberName });
            }

            var total = householdDecimal + nonHouseholdDecimal;
            if (total >= 5m)
            {
                return new ValidationResult("EEE details need to total less than 5 tonnes", new[] { validationContext.MemberName });
            }

            return ValidationResult.Success;
        }
    }
}
