namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using EA.Weee.Xml.MemberRegistration;

    public interface IProducerChargeCalculator
    {
        ProducerCharge CalculateCharge(schemeType scheme, producerType producer, int complianceYear);
    }
}
