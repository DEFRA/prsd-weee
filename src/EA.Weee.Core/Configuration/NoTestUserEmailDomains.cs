namespace EA.Weee.Core.Configuration
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// This collection will be used when no test internal user email domains are defined.
    /// I.e. This will be the default behaviour in the production environment.
    /// </summary>
    public class NoTestUserEmailDomains : ITestUserEmailDomains
    {
        public bool UserTestModeEnabled
        {
            get { return false; }
        }

        public IEnumerable<string> Domains
        {
            get
            {
                return Enumerable.Empty<string>();
            }
        }
    }
}
