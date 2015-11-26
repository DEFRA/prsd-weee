namespace EA.Weee.Xml.MemberRegistration
{
    using Domain.Lookup;

    public interface IProducerChargeBandCalculator
    {
        ChargeBand GetProducerChargeBand(annualTurnoverBandType annualTurnoverBand, bool vatRegistered, eeePlacedOnMarketBandType eeePlacedOnMarketBand);
    }
}
