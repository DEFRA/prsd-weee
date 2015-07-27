namespace EA.Weee.Web.Services.EmailRules
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
    public class RuleChecker
    {
        private Dictionary<string, Regex> regularExpressions = new Dictionary<string, Regex>();

        public RuleAction CheckEmailAddress(string emailAddress)
        {
            RulesSection config = ConfigurationManager.GetSection("emailRules") as RulesSection;

            if (config == null)
            {
                throw new ConfigurationErrorsException("The 'emailRules' section was not present in the configuration file.");
            }

            RuleAction result = config.DefaultAction;

            foreach (RuleElement rule in config.Rules)
            {
                RuleAction? ruleResult = TestRule(rule, emailAddress);
                if (ruleResult != null)
                {
                    result = ruleResult.Value;
                    break;
                }
            }

            return result;
        }

        private RuleAction? TestRule(RuleElement rule, string emailAddress)
        {
            bool isMatch;
            switch (rule.Type)
            {
                case RuleType.EmailAddress:
                    isMatch = (string.Compare(emailAddress, rule.Value, true) == 0);
                    break;

                case RuleType.RegEx:
                    Regex regex = GetRegex(rule.Value);
                    isMatch = regex.IsMatch(emailAddress);
                    break;

                default:
                    throw new NotSupportedException();
            }

            return isMatch ? rule.Action : (RuleAction?)null;
        }

        private Regex GetRegex(string pattern)
        {
            if (regularExpressions.ContainsKey(pattern))
            {
                return regularExpressions[pattern];
            }
            else
            {
                Regex regex = new Regex(pattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
                regularExpressions.Add(pattern, regex);
                return regex;
            }
        }
    }
}
