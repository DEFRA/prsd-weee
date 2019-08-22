namespace EA.Weee.DataAccess.Mappings
{
    using Domain.Producer;
    using System.Data.Entity.ModelConfiguration;

    internal class ProducerPartnershipMapping : EntityTypeConfiguration<Partnership>
    {
        public ProducerPartnershipMapping()
        {
            ToTable("Partnership", "Producer");
        }
    }
}
