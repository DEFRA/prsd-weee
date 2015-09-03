namespace EA.Weee.Core.XmlBusinessValidation
{
    public interface IRule<in T>
    {
        RuleResult Evaluate(T ruleData);
    }
}
