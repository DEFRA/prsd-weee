namespace EA.Weee.Core.Validation
{
    using DataReturns;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Helpers;

    [AttributeUsage(AttributeTargets.Property)]
    public class TonnageValueAttribute : ValidationAttribute
    {
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

            var validator = new TonnageValueValidator();
            var result = validator.Validate(value);

            if (result != TonnageValidationResult.Success)
            {
                switch (result.Type)
                {
                    case TonnageValidationTypeEnum.MaximumDigits:
                        return new ValidationResult(GenerateMessage($"numerical with {TonnageValueValidator.MaxTonnageLength} digits or less",
                            (int)propertyValue));
                    case TonnageValidationTypeEnum.NotNumerical:
                        return new ValidationResult(GenerateMessage("numerical", (int)propertyValue));
                    case TonnageValidationTypeEnum.LessThanZero:
                        return new ValidationResult(GenerateMessage("0 or greater", (int)propertyValue));
                    case TonnageValidationTypeEnum.DecimalPlaces:
                        return new ValidationResult(GenerateMessage("3 decimal places or less", (int)propertyValue));
                    case TonnageValidationTypeEnum.DecimalPlaceFormat:
                        return new ValidationResult(GenerateMessage("entered in its correct format using only a comma as a thousand separator", (int)propertyValue));
                }
            }

            return ValidationResult.Success;
        }
        private string GenerateMessage(string message, int categoryId)
        {
            var category = DisplayCategory ? $"{((WeeeCategory)categoryId).ToCustomDisplayString()} " : string.Empty;

            var additionalMessage = TypeMessage == null ? string.Empty : $"{TypeMessage} ";

            return $"{StartOfValidationMessage} for category {categoryId} {category}{additionalMessage}must be {message}";
        }
    }
}
