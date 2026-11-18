namespace EA.Prsd.Core.Validation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class PasswordComplexityAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var password = value as string ?? string.Empty;
            var errors = new List<string>();

            if (password.Length < 12 ||
                !password.Any(char.IsLower) ||
                !password.Any(char.IsUpper) ||
                !password.Any(char.IsDigit) ||
                !password.Any(c => !char.IsLetterOrDigit(c)))
            {
                errors.Add("The password must be at least 12 characters long");
                errors.Add("The password must contain at least 1 lower-case letter");
                errors.Add("The password must contain at least 1 upper-case letter");
                errors.Add("The password must contain at least 1 numerical value");
                errors.Add("The password must contain at least 1 special character (e.g. !, ?, @, #)");
            }

            return errors.Any() ? new ValidationResult(string.Join(Environment.NewLine, errors)) : ValidationResult.Success;
        }
    }
}
