namespace EA.Weee.Core.Validation
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Text.RegularExpressions;

    [AttributeUsage(AttributeTargets.Property)]
    public class GenericPhoneNumberAttribute : ValidationAttribute
    {
        /// <summary>
        /// Checks that string is compromised only of digits, white spaces and special characters i.e.(+ . - "()") only.
        /// </summary>
        /// <param name="value">The string to check</param>
        /// <returns>A boolean confirming whether the string is a valid phone number</returns>
        public override bool IsValid(object value)
        {
            if (value != null)
            {
                Regex validRegex = new Regex("^[0-9+.()\\s-]+$");
                return validRegex.IsMatch(value.ToString());
            }

            return false;
        }
    }
}