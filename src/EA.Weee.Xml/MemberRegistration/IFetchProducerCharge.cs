namespace EA.Weee.Xml.MemberRegistration
{
    using System.Threading.Tasks;
    using Domain.Lookup;

    public interface IFetchProducerCharge
    {
        Task<ProducerCharge> GetCharge(ChargeBand chargeBand);
    }
}