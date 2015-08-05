namespace EA.Weee.Core.Configuration.EmailRules
{
    public interface IRuleSectionChecker
    {
        RuleAction CheckEmailAddress(string emailAddress);
    }
}