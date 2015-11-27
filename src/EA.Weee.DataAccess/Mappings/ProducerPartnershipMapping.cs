namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Producer;

    internal class ProducerPartnershipMapping : EntityTypeConfiguration<Partnership>
    {
        public ProducerPartnershipMapping()
        {
            ToTable("Partnership", "Producer");
        }
    }
}
