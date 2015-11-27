namespace EA.Weee.Core.Validation
{
    using Configuration;
    using Shared;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class ExternalEmailAddressAttribute : ValidationAttribute
    {
        public ExternalEmailAddressAttribute()
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

            //AA users will not be allowed to create account on external site unless "testInternalUserEmailDomains" is enabled.
            if (InternalDomains.InternalAllowedDomains.Any(notAllowedDomain => string.Equals(notAllowedDomain, domain, StringComparison.OrdinalIgnoreCase)))
            {
                return InternalDomains.TestUserEmailDomains.UserTestModeEnabled;
            }
            // If the domain didn't match any of the internal domains, then the validation passes.
            return true;
        }
    }
}
