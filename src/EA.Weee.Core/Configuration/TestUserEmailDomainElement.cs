﻿namespace EA.Weee.Core.Configuration
{
    using System.Configuration;

    public class TestUserEmailDomainElement : ConfigurationElement
    {
        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get => (string)this["value"];
            set => this["value"] = value;
        }
    }
}
