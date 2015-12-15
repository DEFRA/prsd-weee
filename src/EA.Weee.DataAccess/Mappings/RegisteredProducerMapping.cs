namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Producer;

    internal class RegisteredProducerMapping : EntityTypeConfiguration<RegisteredProducer>
    {
        public RegisteredProducerMapping()
        {
            Ignore(m => m.IsAligned);

            Map(m =>
            {
                m.ToTable("RegisteredProducer", "Producer");
                m.Requires("IsAligned").HasValue(true);
            });
        }
    }
}