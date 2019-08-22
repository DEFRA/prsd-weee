namespace EA.Weee.DataAccess.Mappings
{
    using Domain.Producer;
    using System.Data.Entity.ModelConfiguration;

    internal class RegisteredProducerMapping : EntityTypeConfiguration<RegisteredProducer>
    {
        public RegisteredProducerMapping()
        {
            ToTable("RegisteredProducer", "Producer");
        }
    }
}