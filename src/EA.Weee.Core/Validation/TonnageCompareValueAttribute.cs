namespace EA.Weee.Core.Validation
{
    using DataReturns;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class TonnageCompareValueAttribute : ValidationAttribute
    {
        private string categoryPropertyErrorMessage;

        public string ComparePropertyName { get; set; }

        public string CategoryProperty { get; private set; }

        public TonnageCompareValueAttribute(string category, string compareProperty)
        {
            CategoryProperty = category;
            ComparePropertyName = compareProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(value?.ToString()) || (!value.ToString().Any(char.IsDigit)))
            {
                return ValidationResult.Success;
            }

            var instance = validationContext.ObjectInstance;
            var type = instance.GetType();
            var categoryProperty = type.GetProperty(CategoryProperty);

            if (categoryProperty == null)
            {
                throw new ValidationException($"Property {CategoryProperty} does not exist");
            }

            var categoryId = Enum.GetValues(typeof(WeeeCategory)).Cast<int>().ToList();
            var categoryPropertyValue = (int)categoryProperty.GetValue(validationContext.ObjectInstance, null) as int?;

            categoryPropertyErrorMessage = $"Property {CategoryProperty} should be of type {nameof(WeeeCategory)}";

            if (categoryPropertyValue == null)
            {
                throw new ValidationException(categoryPropertyErrorMessage);
            }

            if (categoryPropertyValue.HasValue && !categoryId.Contains(categoryPropertyValue.Value))
            {
                throw new ValidationException(categoryPropertyErrorMessage);
            }

            var dependentProperty = type.GetProperty(ComparePropertyName);

            if (dependentProperty == null)
            {
                throw new ValidationException($"Compare Property {ComparePropertyName} does not exist");
            }

            var dependentPropertyValue = dependentProperty.GetValue(instance, null);

            if ((decimal.TryParse(value.ToString(), NumberStyles.Number & ~NumberStyles.AllowTrailingSign,
                    CultureInfo.InvariantCulture, out var decimalResult) && (dependentPropertyValue == null || string.IsNullOrWhiteSpace(dependentPropertyValue.ToString()))))
            {
                return new ValidationResult(GenerateMessage());
            }

            if ((decimal.TryParse(dependentPropertyValue.ToString(), NumberStyles.Number & ~NumberStyles.AllowTrailingSign,
                    CultureInfo.InvariantCulture, out var decimalDependentResult)))
            {
                if (decimalResult > decimalDependentResult)
                {
                    return new ValidationResult(GenerateMessage());
                }
            }
            
            return ValidationResult.Success;
        }

        private string GenerateMessage()
        {
            return ErrorMessage;
        }
    }
}
