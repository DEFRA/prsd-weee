namespace EA.Prsd.Email.Rules
{
    using System.Collections.Generic;

    public interface IRuleSet
    {
        RuleAction DefaultAction { get; }

        IEnumerable<IRule> Rules { get; }

        RuleAction Check(string emailAddress);
    }
}