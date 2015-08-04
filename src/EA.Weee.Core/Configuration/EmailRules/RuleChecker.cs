namespace EA.Weee.Core.Configuration.EmailRules
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public class RuleChecker : IRuleChecker
    {
        private readonly Dictionary<string, Regex> regularExpressions = new Dictionary<string, Regex>();

        public RuleAction? Check(RuleElement rule, string emailAddress)
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
