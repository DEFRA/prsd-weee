namespace EA.Weee.Core.Validation
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using DataReturns;
    using DataStandards;

    public class TonnageValueAttribute : ValidationAttribute
    {
        public const int MaxTonnageLength = 14;

        public string CategoryProperty { get; private set; }
        public string TypeMessage { get; private set; }
        
        /* Regex to validate correct use of commas as thousands separator.  Must also consider presence of decimals*/
        private readonly Regex validThousandRegex = new Regex(@"(^\d{1,3}(,\d{3})*\.\d+$)|(^\d{1,3}(,\d{3})*$)|(^(\d)*\.\d*$)|(^\d*$)");

        public TonnageValueAttribute(string category)
        {
            this.CategoryProperty = category;
            this.TypeMessage = null;
        }

        public TonnageValueAttribute(string category, string typeMessage)
        {
            this.CategoryProperty = category;
            this.TypeMessage = typeMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyInfos = validationContext.ObjectType.GetProperties().FirstOrDefault(p => p.Name == this.CategoryProperty);

            if (propertyInfos == null)
            {
                throw new ValidationException($"Property {CategoryProperty} does not exist");
            }

            var propertyValue = (int)propertyInfos.GetValue(validationContext.ObjectInstance, null) as int?;

            var categoryId = Enum.GetValues(typeof(WeeeCategory)).Cast<int>().ToList();

            if (propertyValue == null || !categoryId.Contains(propertyValue.Value))
            {
                throw new ValidationException($"Property {CategoryProperty} should be of type {typeof(WeeeCategory).Name}");
            }

            if (string.IsNullOrWhiteSpace(value?.ToString()))
            {
                return ValidationResult.Success;
            }

            if (Length(value) > MaxTonnageLength)
            {
                return new ValidationResult(GenerateMessage($"numerical with {MaxTonnageLength} digits or less", (int)propertyValue));
            }

            if (!decimal.TryParse(value.ToString(), NumberStyles.Number & ~NumberStyles.AllowTrailingSign, CultureInfo.InvariantCulture, out var decimalResult))
            {
                return new ValidationResult(GenerateMessage("numerical", (int)propertyValue));
            }
            else
            {
                if (decimalResult < 0 || (value.ToString().Substring(0, 1) == "-"))
                {
                    return new ValidationResult(GenerateMessage("0 or greater", (int)propertyValue));
                }

                if (!decimal.TryParse(value.ToString(),
                    NumberStyles.Number &
                    ~NumberStyles.AllowLeadingWhite &
                    ~NumberStyles.AllowTrailingWhite &
                    ~NumberStyles.AllowLeadingSign &
                    ~NumberStyles.AllowTrailingSign,
                    CultureInfo.InvariantCulture,
                    out decimalResult))
                {
                    return new ValidationResult(GenerateMessage("numerical", (int)propertyValue));
                }

                if (decimalResult.DecimalPlaces() > 3)
                {
                    return new ValidationResult(GenerateMessage("3 decimal places or less", (int)propertyValue));
                }
            }

            if (!validThousandRegex.IsMatch(value?.ToString()))
            {
                return new ValidationResult(GenerateMessage("entered correctly.  E.g. 1,000 or 100", (int)propertyValue));
            }

            return ValidationResult.Success;
        }

        private int Length(object value)
        {
            var decimalPlaces = value.ToString().DecimalPlaces();
            var lengthTrimmed = value.ToString().Replace(",", string.Empty).Length;

            if (decimalPlaces > 0)
            {
                return lengthTrimmed - (decimalPlaces + 1);
            }

            return lengthTrimmed;
        }

        private string GenerateMessage(string message, int categoryId)
        {
            var additionalMessage = TypeMessage == null ? string.Empty : $" {TypeMessage}";
            
            return $"The tonnage value for Category {categoryId}{additionalMessage} must be {message}";
        }
    }
}
