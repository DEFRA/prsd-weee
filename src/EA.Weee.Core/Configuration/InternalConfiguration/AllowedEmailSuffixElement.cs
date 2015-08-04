namespace EA.Weee.Core.Configuration.InternalConfiguration
{
    using System.Configuration;

    public class AllowedEmailSuffixElement : ConfigurationElement
    {
        [ConfigurationProperty("value", DefaultValue = "", IsRequired = true, IsKey = true)]
        public string Value
        {
            get
            {
                return (string)this["value"];
            }
            set
            {
                this["value"] = value;
            }
        }
    }
}
