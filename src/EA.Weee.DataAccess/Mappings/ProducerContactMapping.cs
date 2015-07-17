namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using EA.Weee.Domain.Producer;

    internal class ProducerContactMapping : EntityTypeConfiguration<ProducerContact>
    {
        public ProducerContactMapping()
        {
            ToTable("Contact", "Producer");
        }
    }
}
