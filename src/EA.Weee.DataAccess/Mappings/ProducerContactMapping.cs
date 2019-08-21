namespace EA.Weee.DataAccess.Mappings
{
    using EA.Weee.Domain.Producer;
    using System.Data.Entity.ModelConfiguration;

    internal class ProducerContactMapping : EntityTypeConfiguration<ProducerContact>
    {
        public ProducerContactMapping()
        {
            ToTable("Contact", "Producer");
        }
    }
}
