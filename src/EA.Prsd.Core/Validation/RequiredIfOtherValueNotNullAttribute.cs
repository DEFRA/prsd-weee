namespace EA.Prsd.Core.Validation
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class RequiredIfOtherValueNotNullAttribute : ValidationAttribute
    {
        private string DependantPropertyName { get; set; }

        public RequiredIfOtherValueNotNullAttribute(string dependentPropertyName)
        {
            DependantPropertyName = dependentPropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var instance = context.ObjectInstance;

            var type = instance.GetType();

            var dependantPropertyValue = type.GetProperty(DependantPropertyName).GetValue(instance, null);
            var propertyValue = type.GetProperty(context.MemberName).GetValue(instance, null);

            if (dependantPropertyValue != null && propertyValue == null)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}