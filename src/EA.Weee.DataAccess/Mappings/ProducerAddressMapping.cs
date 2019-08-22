namespace EA.Weee.DataAccess.Mappings
{
    using EA.Weee.Domain.Producer;
    using System.Data.Entity.ModelConfiguration;

    internal class ProducerAddressMapping : EntityTypeConfiguration<ProducerAddress>
    {
        public ProducerAddressMapping()
        {
            ToTable("Address", "Producer");
        }
    }
}
