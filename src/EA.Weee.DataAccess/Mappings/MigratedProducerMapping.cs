namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Producer;

    internal class MigratedProducerMapping : EntityTypeConfiguration<MigratedProducer>
    {
        public MigratedProducerMapping()
        {
            ToTable("MigratedProducer", "Producer");
        }
    }
}
