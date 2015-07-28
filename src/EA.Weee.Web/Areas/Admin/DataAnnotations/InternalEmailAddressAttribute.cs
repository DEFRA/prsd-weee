namespace EA.Weee.Web.Areas.Admin.DataAnnotations
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    [AttributeUsage(AttributeTargets.Property)]
    public class InternalEmailAddressAttribute : ValidationAttribute
    {
        private readonly string[] emailSuffixWhitelist = 
        {
            "@environment-agency.gov.uk",
            "@cyfoethnaturiolcymru.gov.uk",
            "@naturalresourceswales.gov.uk",
            "@sepa.org.uk",
            "@doeni.gov.uk"
        };

        public override bool IsValid(object value)
        {
            if (value != null
                && value.ToString() != string.Empty
                && new EmailAddressAttribute().IsValid(value)
                && !emailSuffixWhitelist.Any(suff => value.ToString().ToLowerInvariant().EndsWith(suff)))
            {
                return false;
            }

            return true;
        }
    }
}