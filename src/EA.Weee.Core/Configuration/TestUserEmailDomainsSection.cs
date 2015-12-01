namespace EA.Weee.Core.Configuration
{
    using System.Collections.Generic;
    using System.Configuration;

    /// <summary>
    /// This configuration section allows additional email address domains
    /// to be considered as valid for internal users. This should only be
    /// used for testing.
    /// </summary>
    public class TestUserEmailDomainsSection : ConfigurationSection, ITestUserEmailDomains
    {
        [ConfigurationProperty("userTestModeEnabled", IsRequired = true)]
        public bool UserTestModeEnabled
        {
            get
            {
                return (bool)this["userTestModeEnabled"];
            }
            set
            {
                this["userTestModeEnabled"] = value;
            }
        }

        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        public TestUserEmailDomainElementCollection Domains
        {
            get
            {
                return (TestUserEmailDomainElementCollection)this[string.Empty];
            }
            set
            {
                this[string.Empty] = value;
            }
        }

        IEnumerable<string> ITestUserEmailDomains.Domains
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
