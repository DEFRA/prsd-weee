namespace EA.Weee.DataAccess.Mappings
{
    using Domain.Organisation;
    using System.Data.Entity.ModelConfiguration;

    internal class ProducerBalancingSchemeMapping : EntityTypeConfiguration<ProducerBalancingScheme>
    {
        public ProducerBalancingSchemeMapping()
        {
            ToTable("ProducerBalancingScheme", "Organisation");
            HasKey(p => p.Lock);
        }
    }
}
