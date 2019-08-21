namespace EA.Weee.Xml.MemberRegistration
{
    using Domain.Lookup;
    using System.Threading.Tasks;

    public interface IFetchProducerCharge
    {
        Task<ProducerCharge> GetCharge(ChargeBand chargeBand);
    }
}