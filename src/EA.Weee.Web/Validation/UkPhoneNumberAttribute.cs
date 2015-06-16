namespace EA.Weee.Web.Validation
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Text.RegularExpressions;

    [AttributeUsage(AttributeTargets.Property)]
    public class UkPhoneNumberAttribute : ValidationAttribute
    {
        /// <summary>
        /// Checks that a number:
        ///     - Contains only digits and 
        ///     - Is 11 characters in length (without whitespace or dashes) 
        ///     - Begins with zero
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        // TODO: Move this to PRSD core
        public override bool IsValid(object value)
        {
            if (value != null)
            {
                var phoneNumber = value.ToString().Trim()
                    .Replace(" ", string.Empty)
                    .Replace("-", string.Empty);

                return new Regex(@"^0\d{10}$").IsMatch(phoneNumber);
            }

            return false;
        }
    }
}
