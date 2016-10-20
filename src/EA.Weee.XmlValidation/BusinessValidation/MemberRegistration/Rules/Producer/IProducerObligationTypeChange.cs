namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.Rules.Producer
{
    using System.Threading.Tasks;
    using Xml.MemberRegistration;

    public interface IProducerObligationTypeChange
    {
        Task<RuleResult> Evaluate(producerType producer);
    }
}
