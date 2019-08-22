namespace EA.Weee.Xml.MemberRegistration
{
    using System.Threading.Tasks;

    public interface IProducerChargeBandCalculator
    {
        Task<ProducerCharge> GetProducerChargeBand(schemeType scheme, producerType producer);

        bool IsMatch(schemeType scheme, producerType producer);
    }
}
