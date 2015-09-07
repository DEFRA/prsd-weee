namespace EA.Prsd.Email.Rules.Configuration
{
    using System.Configuration;

    public class RulesSection : ConfigurationSection, IRuleSet
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

        System.Collections.Generic.IEnumerable<IRule> IRuleSet.Rules
        {
            get
            {
                for (int index = 0; index < Rules.Count; index++)
                {
                    yield return Rules[index];
                }
            }
        }

        public RuleAction Check(string emailAddress)
        {
            RuleAction result = DefaultAction;

            foreach (IRule rule in Rules)
            {
                RuleAction? ruleResult = rule.Check(emailAddress);
                if (ruleResult != null)
                {
                    result = ruleResult.Value;
                    break;
                }
            }

            return result;
        }
    }
}
