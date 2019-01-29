namespace EA.Weee.Core.Validation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text.RegularExpressions;
    using DataReturns;

    public class TonnageValueAttribute : ValidationAttribute
    {
        private readonly string categoryProperty;

        public TonnageValueAttribute(string category)
        {
            this.categoryProperty = category;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyInfos = validationContext.ObjectType.GetProperties().FirstOrDefault(p => p.Name == this.categoryProperty);

            if (propertyInfos == null)
            {
                throw new ValidationException($"Property {categoryProperty} does not exist");
            }

            var propertyValue = (int)propertyInfos.GetValue(validationContext.ObjectInstance, null) as int?;

            var categoryId = Enum.GetValues(typeof(WeeeCategory)).Cast<int>().ToList();

            if (propertyValue == null || !categoryId.Contains(propertyValue.Value))
            {
                throw new ValidationException($"Property {categoryProperty} should be of type {typeof(WeeeCategory).Name}");
            }

            if (value is null)
            {
                return ValidationResult.Success;
            }

            if (!decimal.TryParse(value.ToString(), out var decimalResult))
            {
                return new ValidationResult($"Category {(int)propertyValue} tonnage value must be a numerical value");
            }
            else
            {
                if (decimalResult < 0)
                {
                    return new ValidationResult($"Category {(int)propertyValue} tonnage value must be 0 or greater");
                }

                if (DecimalPlaces(decimalResult) > 3)
                {
                    return new ValidationResult($"Category {(int)propertyValue} tonnage value must be 3 decimal places or less");
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
