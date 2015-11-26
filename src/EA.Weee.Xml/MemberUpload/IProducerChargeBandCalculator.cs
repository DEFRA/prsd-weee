namespace EA.Weee.Xml.MemberUpload
{
    using Domain.Lookup;

    public interface IProducerChargeBandCalculator
    {
        ChargeBand GetProducerChargeBand(annualTurnoverBandType annualTurnoverBand, bool vatRegistered, eeePlacedOnMarketBandType eeePlacedOnMarketBand);
    }
}
