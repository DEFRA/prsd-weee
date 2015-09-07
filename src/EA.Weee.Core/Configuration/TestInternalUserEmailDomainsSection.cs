namespace EA.Weee.Core.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This configuration section allows additional email address domains
    /// to be considered as valid for internal users. This should only be
    /// used for testing.
    /// </summary>
    public class TestInternalUserEmailDomainsSection : ConfigurationSection, ITestInternalUserEmailDomains
    {
        [ConfigurationProperty("enabled", IsRequired = true)]
        public bool Enabled
        {
            get
            {
                return (bool)this["enabled"];
            }
            set
            {
                this["enabled"] = value;
            }
        }

        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        public TestInternalUserEmailDomainElementCollection Domains
        {
            get
            {
                return (TestInternalUserEmailDomainElementCollection)this[string.Empty];
            }
            set
            {
                this[string.Empty] = value;
            }
        }

        IEnumerable<string> ITestInternalUserEmailDomains.Domains
        {
            get
            {
                for (int index = 0; index < Domains.Count; index++)
                {
                    yield return Domains[index].Value;
                }
            }
        }
    }
}
