namespace EA.Weee.Core.Validation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;

    public class AtLeastOneCheckedAttribute : ValidationAttribute
    {
        private readonly string[] propertyNamesToCheck;
        private readonly string validationPropertyName;

        public AtLeastOneCheckedAttribute(string validationPropertyName, params string[] propertyNames)
        {
            this.validationPropertyName = validationPropertyName ?? throw new ArgumentNullException(nameof(validationPropertyName));
            this.propertyNamesToCheck = propertyNames ?? throw new ArgumentNullException(nameof(propertyNames));

            if (this.propertyNamesToCheck.Length == 0)
            {
                throw new ArgumentException("At least one property name must be provided.", nameof(propertyNames));
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var containerObject = GetContainerObject(validationContext);
            if (containerObject == null)
            {
                return new ValidationResult(GetErrorMessage());
            }

            var checkedProperties = GetCheckedBooleanProperties(containerObject);
            return checkedProperties.Any()
                ? ValidationResult.Success
                : new ValidationResult(GetErrorMessage(), new[] { validationPropertyName });
        }

        private object GetContainerObject(ValidationContext validationContext)
        {
            var containerProperty = validationContext.ObjectType.GetProperty(validationPropertyName);
            if (containerProperty == null)
            {
                throw new InvalidOperationException($"Property '{validationPropertyName}' not found on {validationContext.ObjectType.Name}.");
            }

            return containerProperty.GetValue(validationContext.ObjectInstance);
        }

        private IEnumerable<PropertyInfo> GetCheckedBooleanProperties(object containerObject)
        {
            return propertyNamesToCheck
                .Select(name => containerObject.GetType().GetProperty(name))
                .Where(prop => prop != null && prop.PropertyType == typeof(bool) && (bool)prop.GetValue(containerObject));
        }

        private string GetErrorMessage()
        {
            return ErrorMessage ?? "At least one option must be selected.";
        }
    }
}
