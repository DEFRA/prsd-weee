namespace EA.Weee.Core.Validation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text.RegularExpressions;
    using DataReturns;
    using DataStandards;

    public class TonnageValueAttribute : ValidationAttribute
    {
        private readonly string categoryProperty;
        private readonly string typeMessage;

        public TonnageValueAttribute(string category)
        {
            this.categoryProperty = category;
            this.typeMessage = null;
        }

        public TonnageValueAttribute(string category, string typeMessage)
        {
            this.categoryProperty = category;
            this.typeMessage = typeMessage;
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

            if (value.ToString().Length > CommonMaxFieldLengths.Tonnage)
            {
                return new ValidationResult(GenerateMessage("a numerical value with 15 digits or less", (int)propertyValue));
            }

            if (!decimal.TryParse(value.ToString(), out var decimalResult))
            {
                return new ValidationResult(GenerateMessage("a numerical value", (int)propertyValue));
            }
            else
            {
                if (decimalResult < 0)
                {
                    return new ValidationResult(GenerateMessage("0 or greater", (int)propertyValue));
                }

                if (DecimalPlaces(decimalResult) > 3)
                {
                    return new ValidationResult(GenerateMessage("3 decimal places or less", (int)propertyValue));
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

        private string GenerateMessage(string message, int categoryId)
        {
            var additionalMessage = typeMessage == null ? string.Empty : $" {typeMessage}";
            
            return $"Category {categoryId}{additionalMessage} tonnage value must be {message}";
        }
    }
}
