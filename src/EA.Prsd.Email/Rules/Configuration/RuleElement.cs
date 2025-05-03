namespace EA.Prsd.Email.Rules.Configuration
{
    using System;
    using System.Configuration;

    public class RuleElement : ConfigurationElement, IRule
    {
        [ConfigurationProperty("action", IsRequired = true)]
        public RuleAction Action
        {
            get => (RuleAction)this["action"];
            set => this["action"] = value;
        }

        [ConfigurationProperty("type", IsRequired = true)]
        public RuleTypeEnum Type
        {
            get => (RuleTypeEnum)this["type"];
            set => this["type"] = value;
        }

        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get => (string)this["value"];
            set => this["value"] = value;
        }

        public enum RuleTypeEnum
        {
            EmailAddress,
            RegEx
        }

        RuleType IRule.Type
        {
            get
            {
                switch (Type)
                {
                    case RuleTypeEnum.EmailAddress:
                        return RuleType.EmailAddress;

                    case RuleTypeEnum.RegEx:
                        return RuleType.RegEx;

                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public RuleAction? Check(string emailAddress)
        {
            return ((IRule)this).Type.Check(this, emailAddress);
        }
    }
}
