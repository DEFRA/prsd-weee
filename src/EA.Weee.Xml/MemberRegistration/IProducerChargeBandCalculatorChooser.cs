namespace EA.Weee.Xml.MemberRegistration
{
    using System.Threading.Tasks;
    using Domain.Lookup;

    public interface IProducerChargeBandCalculatorChooser
    {
        Task<ProducerCharge> GetProducerChargeBand(schemeType scheme, producerType producer);
    }
}