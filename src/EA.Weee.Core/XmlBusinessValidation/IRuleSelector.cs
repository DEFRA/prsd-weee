namespace EA.Weee.Core.XmlBusinessValidation
{
    public interface IRuleSelector
    {
        RuleResult EvaluateRule<T>(T rule);
    }
}
