namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using EA.Weee.Domain.Producer;

    internal class ProducerAddressMapping : EntityTypeConfiguration<ProducerAddress>
    {
        public ProducerAddressMapping()
        {
            ToTable("Address", "Producer");
        }
    }
}
