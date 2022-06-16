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
        public string ComparePropertyName { get; private set; }

        public string CategoryProperty { get; private set; }

        public bool DisplayCategory { get; private set; }

        public TonnageCompareValueAttribute(string category, string compareProperty, string errorMessage, bool displayCategory = false)
        {
            CategoryProperty = category;
            ComparePropertyName = compareProperty;
            ErrorMessage = errorMessage;
            DisplayCategory = displayCategory;
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

            var categoryPropertyErrorMessage = $"Property {CategoryProperty} should be of type {nameof(WeeeCategory)}";

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
                    CultureInfo.InvariantCulture, out var decimalResult)))
            {
                if (dependentPropertyValue == null || string.IsNullOrWhiteSpace(dependentPropertyValue.ToString()))
                {
                    if (decimalResult != 0)
                    {
                        return new ValidationResult(GenerateMessage(categoryPropertyValue.Value));
                    }
                }

                if (dependentPropertyValue != null)
                {
                    if ((decimal.TryParse(dependentPropertyValue.ToString(), NumberStyles.Number & ~NumberStyles.AllowTrailingSign,
                            CultureInfo.InvariantCulture, out var decimalDependentResult)))
                    {
                        if (decimalResult > decimalDependentResult)
                        {
                            return new ValidationResult(GenerateMessage(categoryPropertyValue.Value));
                        }
                    }
                }
            }

            return ValidationResult.Success;
        }

        public override object TypeId => this;

        private string GenerateMessage(int categoryId)
        {
            if (DisplayCategory)
            {
                var category = $"{categoryId} {((WeeeCategory)categoryId).ToCustomDisplayString()}";

                return string.Format(ErrorMessage, category);
            }

            return ErrorMessage;
        }
    }
}
