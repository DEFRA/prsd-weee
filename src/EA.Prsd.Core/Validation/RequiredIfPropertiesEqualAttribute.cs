namespace EA.Prsd.Core.Validation
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class RequiredIfPropertiesEqualAttribute : ValidationAttribute
    {
        private string DependantPropertyName { get; set; }
        private string PropertyName { get; set; }

        public RequiredIfPropertiesEqualAttribute(string dependentPropertyName, string propertyName)
        {
            DependantPropertyName = dependentPropertyName;
            PropertyName = propertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var instance = context.ObjectInstance;
            var type = instance.GetType();

            var dependantPropertyValue = type.GetProperty(DependantPropertyName).GetValue(instance, null);
            var propertyValue = type.GetProperty(PropertyName).GetValue(instance, null);
            var contextPropertyValue = type.GetProperty(context.MemberName).GetValue(instance, null);

            if (dependantPropertyValue != null && (dependantPropertyValue.ToString().Equals(propertyValue.ToString())))
            {
                if (contextPropertyValue == null || string.IsNullOrEmpty(contextPropertyValue.ToString()))
                {
                    return new ValidationResult(ErrorMessage);
                }
            }
            return ValidationResult.Success;
        }
    }
}