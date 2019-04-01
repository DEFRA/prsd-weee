namespace EA.Weee.Sroc.Migration.OverrideImplementations
{
    using System.Threading.Tasks;
    using Domain.Scheme;
    using Xml.MemberRegistration;

    public interface IMigrationProducerChargeBandCalculatorChooser
    {
        Task<ProducerCharge> GetProducerChargeBand(schemeType scheme, producerType producer, MemberUpload upload, string name);
    }
}