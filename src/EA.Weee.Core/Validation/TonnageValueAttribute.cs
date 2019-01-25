namespace EA.Weee.Core.Validation
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Text.RegularExpressions;

    public class TonnageValueAttribute : ValidationAttribute
    {
        private readonly int category;

        public TonnageValueAttribute(int category)
        {
            this.category = category;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return ValidationResult.Success;
            }

            if (!decimal.TryParse(value.ToString(), out var decimalResult))
            {
                return new ValidationResult($"Category {category} tonnage value must be a numerical value");
            }
            else
            {
                if (decimalResult < 0)
                {
                    return new ValidationResult($"Category {category} tonnage value must be 0 or greater");
                }

                if (DecimalPlaces(decimalResult) > 3)
                {
                    return new ValidationResult($"Category {category} tonnage value must be 3 decimal places or less");
                }
            }

            return ValidationResult.Success;
        }

        private int DecimalPlaces(decimal value)
        {
            if (value == 0)
            {
                return 0;
            }
            else
            {
                var bits = decimal.GetBits(value);
                var exponent = bits[3] >> 16;
                var result = exponent;
                long lowDecimal = bits[0] | (bits[1] >> 8);

                while ((lowDecimal % 10) == 0)
                {
                    result--;
                    lowDecimal /= 10;
                }

                return result;
            }
        }
    }
}
