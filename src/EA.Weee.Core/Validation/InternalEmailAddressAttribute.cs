namespace EA.Weee.Core.Validation
{
    using Configuration;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Shared;

    [AttributeUsage(AttributeTargets.Property)]
    public class InternalEmailAddressAttribute : ValidationAttribute
    {
        public InternalEmailAddressAttribute()
        {
            ConfigurationManagerWrapper configuration = new ConfigurationManagerWrapper();
            InternalDomains.TestUserEmailDomains = configuration.TestInternalUserEmailDomains;
        }

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
                    "The InternalEmailAddress attribute can only be " +
                    "applied to properties whose value is a string.");
            }

            string emailAddress = (string)value;

            if (!InternalDomains.EmailRegex.IsMatch(emailAddress))
            {
                // If the value is not a valid email address then it will not be validated.
                return true;
            }
            
            string domain = emailAddress.Split('@')[1];

            if (InternalDomains.InternalAllowedDomains.Any(allowedDomain => string.Equals(allowedDomain, domain, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            if (InternalDomains.TestUserEmailDomains.UserTestModeEnabled)
            {
                return InternalDomains.TestUserEmailDomains.Domains.Any(allowedDomain => string.Equals(allowedDomain, domain, StringComparison.OrdinalIgnoreCase));
            }

            // If the domain didn't match any of the allowed domains, then the validation fails.
            return false;
        }
    }
}