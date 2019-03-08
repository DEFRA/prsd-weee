namespace EA.Weee.Xml.MemberRegistration
{
    using Domain.Lookup;    

    public class EnvironmentAgencyProducerChargeBandCalculator : IEnvironmentAgencyProducerChargeBandCalculator
    {
        public ChargeBand GetProducerChargeBand(producerType producerType)
        {
            return ChargeBand.A2;
        }
    }
}
