namespace EA.Weee.DataAccess.Mappings
{
    using Domain.Producer;
    using System.Data.Entity.ModelConfiguration;

    internal class MigratedProducerMapping : EntityTypeConfiguration<MigratedProducer>
    {
        public MigratedProducerMapping()
        {
            ToTable("MigratedProducer", "Producer");
        }
    }
}
