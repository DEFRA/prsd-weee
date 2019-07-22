namespace EA.Weee.Sroc.Migration.OverrideImplementations
{
    using Domain.Lookup;
    using Xml.MemberRegistration;

    public interface IMigrationFetchProducerCharge
    {
        ProducerCharge GetCharge(ChargeBand chargeBand);
    }
}
