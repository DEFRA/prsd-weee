namespace EA.Weee.Core.Validation
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Configuration;
    using Configuration.EmailRules;

    [AttributeUsage(AttributeTargets.Property)]
    public class InternalEmailAddressAttribute : ValidationAttribute
    {
        private readonly IRuleChecker ruleChecker;
        public IConfigurationManagerWrapper Configuration { get; set; }

        public InternalEmailAddressAttribute()
        {
            Configuration = new ConfigurationManagerWrapper();
            ruleChecker = new RuleChecker();
        }

        public override bool IsValid(object value)
        {
            var emailRules = Configuration.InternalEmailRules.Rules
                .Cast<RuleElement>();

            if (value != null
                && value.ToString() != string.Empty
                && new EmailAddressAttribute().IsValid(value)
                && emailRules.All(r =>
                {
                    var action = ruleChecker.Check(r, value.ToString());
                    if (action == null || action == RuleAction.Deny)
                    {
                        return true;
                    }

                    return false;
                }))
            {
                return false;
            }

            return true;
        }
    }
}