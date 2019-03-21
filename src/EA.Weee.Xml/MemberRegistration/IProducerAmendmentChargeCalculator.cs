namespace EA.Weee.Xml.MemberRegistration
{
    using System.Threading.Tasks;
    using Domain.Lookup;

    public interface IProducerAmendmentChargeCalculator
    {
        Task<ChargeBand?> GetChargeAmount(schemeType schmemeType, producerType producerType);
    }
}
