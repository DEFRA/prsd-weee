namespace EA.Weee.Core.Configuration.EmailRules
{
    public interface IRuleChecker
    {
        RuleAction? Check(RuleElement rule, string emailAddress);
    }
}
