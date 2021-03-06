﻿namespace EA.Weee.Core.Validation
{
    using System.Collections;
    using System.ComponentModel.DataAnnotations;

    public class MinimumElementsAttribute : ValidationAttribute
    {
        private readonly int minElements;
        private readonly string errorMessage;

        public MinimumElementsAttribute(int minElements, string errorMessage)
        {
            this.minElements = minElements;
            this.errorMessage = errorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var list = value as IList;

            var result = list?.Count >= minElements;

            return result
                ? ValidationResult.Success
                : new ValidationResult(errorMessage);
        }
    }
}
