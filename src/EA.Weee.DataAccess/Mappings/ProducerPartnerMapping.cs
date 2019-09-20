namespace EA.Weee.DataAccess.Mappings
{
    using Domain.Producer;
    using System.Data.Entity.ModelConfiguration;

    internal class ProducerPartnerMapping : EntityTypeConfiguration<Partner>
    {
        public ProducerPartnerMapping()
        {
            ToTable("Partner", "Producer");
        }
    }
}