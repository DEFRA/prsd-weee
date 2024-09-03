namespace EA.Weee.Core.Validation
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Text.RegularExpressions;

    [AttributeUsage(AttributeTargets.Property)]
    public class GenericPhoneNumberAttribute : ValidationAttribute
    {
        public bool AllowNull { get; set; }

        public GenericPhoneNumberAttribute(bool allowNull = false)
        {
            AllowNull = allowNull;
        }

        /// <summary>
        /// Checks that string is comprised only of digits, white spaces and special characters i.e.(+ . - "()") only.
        /// If AllowNull is true, null values are considered valid.
        /// </summary>
        /// <param name="value">The string to check</param>
        /// <returns>A boolean confirming whether the string is a valid phone number</returns>
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return AllowNull;
            }

            if (value is string phoneNumber)
            {
                Regex validRegex = new Regex(@"^[0-9+.()\s-]+$");
                return validRegex.IsMatch(phoneNumber);
            }

            return false;
        }
    }
}