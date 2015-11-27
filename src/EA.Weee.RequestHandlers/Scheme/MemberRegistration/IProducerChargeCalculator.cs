namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using EA.Weee.Xml.MemberRegistration;

    public interface IProducerChargeCalculator
    {
        ProducerCharge CalculateCharge(string schemeApprovalNumber, producerType producer, int complianceYear);
    }
}
