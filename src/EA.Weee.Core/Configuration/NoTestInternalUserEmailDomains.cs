namespace EA.Weee.Core.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This collection will be used when no test internal user email domains are defined.
    /// I.e. This will be the default behaviour in the production environment.
    /// </summary>
    public class NoTestInternalUserEmailDomains : ITestInternalUserEmailDomains
    {
        public bool Enabled
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
