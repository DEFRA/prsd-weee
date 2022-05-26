namespace EA.Weee.Core.Validation
{
    using DataReturns;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web.Mvc;
    using Helpers;

    [AttributeUsage(AttributeTargets.Property)]
    public class TonnageValueAttribute : ValidationAttribute
    {
        public const int MaxTonnageLength = 14;

        public string CategoryProperty { get; private set; }
        public string TypeMessage { get; private set; }

        public string StartOfValidationMessage { get; private set; }

        public bool DisplayCategory { get; private set; }

        public TonnageValueAttribute(string category, string startOfValidationMessage, bool displayCategory)
        {
            CategoryProperty = category;
            TypeMessage = null;
            StartOfValidationMessage = startOfValidationMessage;
            DisplayCategory = displayCategory;
        }

        public TonnageValueAttribute(string category, string startOfValidationMessage, string typeMessage, bool displayCategory)
        {
            CategoryProperty = category;
            TypeMessage = typeMessage;
            StartOfValidationMessage = startOfValidationMessage;
            DisplayCategory = displayCategory;
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
                throw new ValidationException($"Property {CategoryProperty} should be of type {nameof(WeeeCategory)}");
            }

            if (string.IsNullOrWhiteSpace(value?.ToString()))
            {
                return ValidationResult.Success;
            }

            if (value.ToString().WeeeDecimalLength())
            {
                return new ValidationResult(GenerateMessage($"numerical with {MaxTonnageLength} digits or less", (int)propertyValue));
            }

            if (!value.ToString().WeeeDecimal(out var decimalResult))
            {
                return new ValidationResult(GenerateMessage("numerical", (int)propertyValue));
            }

            if (value.ToString().WeeeNegativeDecimal(decimalResult))
            {
                return new ValidationResult(GenerateMessage("0 or greater", (int)propertyValue));
            }

            if (!value.ToString().WeeeDecimalWithWhiteSpace(out var decimalResult2))
            {
                return new ValidationResult(GenerateMessage("numerical", (int)propertyValue));
            }

            if (decimalResult2.WeeeDecimalThreePlaces())
            {
                return new ValidationResult(GenerateMessage("3 decimal places or less", (int)propertyValue));
            }

            if (value.WeeeThousandSeparator())
            {
                return new ValidationResult(GenerateMessage("entered in its correct format using only a comma as a thousand separator", (int)propertyValue));
            }

            return ValidationResult.Success;
        }

        private string GenerateMessage(string message, int categoryId)
        {
            var category = DisplayCategory ? $"{((WeeeCategory)categoryId).ToDisplayString().ToLower()} " : string.Empty;

            var additionalMessage = TypeMessage == null ? string.Empty : $"{TypeMessage} ";

            return $"{StartOfValidationMessage} for category {categoryId} {category}{additionalMessage}must be {message}";
        }
    }
}
