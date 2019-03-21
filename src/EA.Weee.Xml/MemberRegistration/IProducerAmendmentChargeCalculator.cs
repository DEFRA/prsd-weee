namespace EA.Weee.Xml.MemberRegistration
{
    using Domain.Lookup;

    public interface IProducerAmendmentChargeCalculator
    {
        ChargeBand? GetChargeAmount(schemeType schmemeType, producerType producerType);
    }
}
