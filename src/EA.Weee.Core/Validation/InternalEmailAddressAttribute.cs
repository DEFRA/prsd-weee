namespace EA.Weee.Core.Validation
{
    using Configuration;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
using System.Text.RegularExpressions;

    [AttributeUsage(AttributeTargets.Property)]
    public class InternalEmailAddressAttribute : ValidationAttribute
    {
        private readonly List<string> allowedDomains = new List<string>()
        {
            "environment-agency.gov.uk",
            "cyfoethnaturiolcymru.gov.uk",
            "naturalresourceswales.gov.uk",
            "sepa.org.uk",
            "doeni.gov.uk"
        };

        private readonly Regex emailRegex = new Regex(@"[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

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
            
            if (!emailRegex.IsMatch(emailAddress))
            {
                // If the value is not a valid email address then it will not be validated.
                return true;
            }
            
            string domain = emailAddress.Split('@')[1];

            foreach (string allowedDomain in allowedDomains)
            {
                if (string.Equals(allowedDomain, domain, StringComparison.OrdinalIgnoreCase))
                {
                    // If the domain matches one of the allowed domains, then the validation passes.
                    return true;
                }
            }

            // If the domain didn't match any of the allowed domains, then the validation fails.
            return false;
        }
    }
}