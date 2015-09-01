namespace EA.Weee.Core.XmlBusinessValidation
{
    public interface IRuleSelector
    {
        IRule<T> GetRule<T>();
    }
}
