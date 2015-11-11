namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Producer;

    internal class ProducerMapping : EntityTypeConfiguration<Producer>
    {
        public ProducerMapping()
        {
            ToTable("Producer", "Producer");
            Ignore(p => p.OrganisationName);
            Property(p => p.AnnualTurnover).HasPrecision(28, 12);
        }
    }
}