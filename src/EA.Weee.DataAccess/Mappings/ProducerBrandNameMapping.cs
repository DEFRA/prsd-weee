namespace EA.Weee.DataAccess.Mappings
{
    using Domain.Producer;
    using System.Data.Entity.ModelConfiguration;

    internal class ProducerBrandNameMapping : EntityTypeConfiguration<BrandName>
    {
        public ProducerBrandNameMapping()
        {
            ToTable("BrandName", "Producer");
        }
    }
}