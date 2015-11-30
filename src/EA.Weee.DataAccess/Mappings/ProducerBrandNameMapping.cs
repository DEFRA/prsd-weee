namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Producer;

    internal class ProducerBrandNameMapping : EntityTypeConfiguration<BrandName>
    {
        public ProducerBrandNameMapping()
        {
            ToTable("BrandName", "Producer");
        }
    }
}