using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace EA.Prsd.Email.Rules
{
    public abstract class RuleType
    {
        public RuleAction? Check(IRule rule, string emailAddress)
        {
            bool isMatch = IsMatch(rule, emailAddress);
            
            return isMatch ? rule.Action : (RuleAction?)null;
        }

        protected abstract bool IsMatch(IRule rule, string emailAddress);

        public static RuleType EmailAddress = new EmailAddressRuleType();

        public static RuleType RegEx = new RegExRuleType();

        protected RuleType()
        {
        }

        private class EmailAddressRuleType : RuleType
        {
            protected override bool IsMatch(IRule rule, string emailAddress)
            {
                return string.Compare(emailAddress, rule.Value, StringComparison.OrdinalIgnoreCase) == 0;
            }
        }

        private class RegExRuleType : RuleType
        {
            private readonly Dictionary<string, Regex> regularExpressions = new Dictionary<string, Regex>();

            protected override bool IsMatch(IRule rule, string emailAddress)
            {
                Regex regex = GetRegex(rule.Value);
                return regex.IsMatch(emailAddress);
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
}
