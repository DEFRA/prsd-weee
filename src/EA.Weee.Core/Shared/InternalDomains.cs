namespace EA.Weee.Core.Shared
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Configuration;

    public static class InternalDomains
    {
        public static readonly List<string> InternalAllowedDomains = new List<string>()
        {
            "environment-agency.gov.uk",
            "cyfoethnaturiolcymru.gov.uk",
            "naturalresourceswales.gov.uk",
            "sepa.org.uk",
            "doeni.gov.uk"
        };

        public static readonly Regex EmailRegex = new Regex(@"[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static ITestUserEmailDomains TestUserEmailDomains;
    }
}
