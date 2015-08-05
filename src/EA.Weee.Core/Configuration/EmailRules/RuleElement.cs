namespace EA.Weee.Core.Configuration.EmailRules
{
    using System.Configuration;

    public class RuleElement : ConfigurationElement
    {
        [ConfigurationProperty("action", IsRequired = true)]
        public RuleAction Action
        {
            get
            {
                return (RuleAction)this["action"];
            }
            set
            {
                this["action"] = value;
            }
        }

        [ConfigurationProperty("type", IsRequired = true)]
        public RuleType Type
        {
            get
            {
                return (RuleType)this["type"];
            }
            set
            {
                this["type"] = value;
            }
        }

        [ConfigurationProperty("value", IsRequired = true)]
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
