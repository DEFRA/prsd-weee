namespace EA.Weee.Core.Configuration.EmailRules
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Provides a way of checking email address against a complex set of whitelist and blacklist rules.
    /// The list of rules are implemented in the "rules" section of the configuration file.
    /// Rules are matched in the order they are defined so the most specific rules should be implemented first.
    /// </summary>
    public class RuleSectionChecker : IRuleSectionChecker
    {
        private readonly IRuleChecker ruleChecker;

        public RuleSectionChecker(IRuleChecker ruleChecker)
        {
            this.ruleChecker = ruleChecker;
        }

        public RuleAction CheckEmailAddress(string emailAddress)
        {
            var config = ConfigurationManager.GetSection("emailRules") as RulesSection;

            if (config == null)
            {
                throw new ConfigurationErrorsException("The 'emailRules' section was not present in the configuration file.");
            }

            RuleAction result = config.DefaultAction;

            foreach (RuleElement rule in config.Rules)
            {
                RuleAction? ruleResult = ruleChecker.Check(rule, emailAddress);
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
