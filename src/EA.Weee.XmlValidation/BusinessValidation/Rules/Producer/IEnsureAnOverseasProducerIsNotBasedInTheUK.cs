namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Producer
{
    using EA.Weee.Xml.MemberRegistration;

    /// <summary>
    /// This rules ensures that an overseas producer does not have an address based in the UK.
    /// </summary>
    public interface IEnsureAnOverseasProducerIsNotBasedInTheUK
    {
        RuleResult Evaluate(producerType producer);
    }
}
