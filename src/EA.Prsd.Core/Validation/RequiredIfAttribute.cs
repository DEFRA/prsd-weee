namespace EA.Prsd.Core.Validation
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class RequiredIfAttribute : ValidationAttribute
    {
        private String DependantPropertyName { get; set; }
        private new String ErrorMessage { get; set; }
        private Object DesiredValue { get; set; }

        public RequiredIfAttribute(String dependentPropertyName, Object desiredValue, String errorMessage)
        {
            DependantPropertyName = dependentPropertyName;
            DesiredValue = desiredValue;
            ErrorMessage = errorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var instance = context.ObjectInstance;
            var type = instance.GetType();
            var dependantPropertyValue = type.GetProperty(DependantPropertyName).GetValue(instance, null);
            var propertyValue = type.GetProperty(context.MemberName).GetValue(instance, null);
            if (dependantPropertyValue != null && (dependantPropertyValue.ToString() == DesiredValue.ToString()))
            {
                if (propertyValue == null || string.IsNullOrEmpty(propertyValue.ToString()))
                {
                    return new ValidationResult(ErrorMessage);
                }
            }
            return ValidationResult.Success;
        }
    }
}