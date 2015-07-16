namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Producer;

    internal class ProducerPartnerMapping : EntityTypeConfiguration<Partner>
    {
        public ProducerPartnerMapping()
        {
            ToTable("Partner", "Producer");
        }
    }
}