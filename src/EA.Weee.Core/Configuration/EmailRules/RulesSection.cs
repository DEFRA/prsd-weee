namespace EA.Weee.Core.Configuration.EmailRules
{
    using System.Configuration;

    public class RulesSection : ConfigurationSection
    {
        [ConfigurationProperty("defaultAction", DefaultValue = RuleAction.Allow, IsRequired = true)]
        public RuleAction DefaultAction
        {
            get
            {
                return (RuleAction)this["defaultAction"];
            }
            set
            {
                this["defaultAction"] = value;
            }
        }

        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        public RuleElementCollection Rules
        {
            get
            {
                return (RuleElementCollection)this[string.Empty];
            }
            set
            {
                this[string.Empty] = value;
            }
        }
    }
}
