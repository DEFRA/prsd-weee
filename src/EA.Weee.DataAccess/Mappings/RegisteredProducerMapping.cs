namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Producer;

    internal class RegisteredProducerMapping : EntityTypeConfiguration<RegisteredProducer>
    {
        public RegisteredProducerMapping()
        {
            ToTable("RegisteredProducer", "Producer");
        }
    }
}