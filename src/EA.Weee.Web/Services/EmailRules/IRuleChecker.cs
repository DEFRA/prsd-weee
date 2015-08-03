namespace EA.Weee.Web.Services.EmailRules
{
    public interface IRuleChecker
    {
        RuleAction CheckEmailAddress(string emailAddress);
    }
}