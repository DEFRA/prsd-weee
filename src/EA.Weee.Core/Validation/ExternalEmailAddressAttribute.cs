namespace EA.Weee.Core.Validation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Shared;

    public class ExternalEmailAddressAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                // Null values will not be validated.
                return true;
            }

            if (!(value is string))
            {
                throw new InvalidOperationException(
                    "The  ExternalEmailAddress attribute can only be " +
                    "applied to properties whose value is a string.");
            }

            string emailAddress = (string)value;

            if (!InternalDomains.EmailRegex.IsMatch(emailAddress))
            {
                // If the value is not a valid email address then it will not be validated.
                return true;
            }

            string domain = emailAddress.Split('@')[1];

            if (InternalDomains.InternalAllowedDomains.Any(notAllowedDomain => string.Equals(notAllowedDomain, domain, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }
            
            // If the domain didn't match any of the internal domains, then the validation passes.
            return true;
        }
    }
}
