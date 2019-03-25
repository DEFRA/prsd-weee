namespace EA.Weee.Xml.MemberRegistration
{
    using System.Threading.Tasks;
    using Domain.Lookup;
   
    public interface IProducerChargeBandCalculator
    {
        Task<ChargeBand> GetProducerChargeBand(schemeType scheme, producerType producer);

        bool IsMatch(schemeType scheme, producerType producer);
    }
}
