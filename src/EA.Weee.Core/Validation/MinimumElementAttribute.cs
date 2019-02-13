namespace EA.Weee.Core.Validation
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

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
