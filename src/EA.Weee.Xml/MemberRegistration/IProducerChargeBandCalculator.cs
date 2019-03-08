namespace EA.Weee.Xml.MemberRegistration
{
    using Domain.Lookup;
   
    public interface IProducerChargeBandCalculator
    {
        ChargeBand GetProducerChargeBand(producerType producerType);
    }
}
