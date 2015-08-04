namespace EA.Weee.Core.Validation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Configuration;
    using Configuration.InternalConfiguration;

    [AttributeUsage(AttributeTargets.Property)]
    public class InternalEmailAddressAttribute : ValidationAttribute
    {
        public IConfigurationManagerWrapper Configuration { get; set; }

        public InternalEmailAddressAttribute()
        {
            Configuration = new ConfigurationManagerWrapper();
        }

        public override bool IsValid(object value)
        {
            var emailSuffixWhiteList = Configuration.InternalConfiguration.AllowedEmailSuffixes
                .Cast<AllowedEmailSuffixElement>()
                .Select(s => s.Value);

            if (value != null
                && value.ToString() != string.Empty
                && new EmailAddressAttribute().IsValid(value)
                && !emailSuffixWhiteList.Any(suff => value.ToString().ToLowerInvariant().EndsWith(suff)))
            {
                return false;
            }

            return true;
        }
    }
}