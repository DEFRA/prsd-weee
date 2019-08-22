namespace EA.Weee.DataAccess.Mappings
{
    using Domain.DataReturns;
    using System.Data.Entity.ModelConfiguration;

    internal class AatfDeliveryLocationMapping : EntityTypeConfiguration<AatfDeliveryLocation>
    {
        public AatfDeliveryLocationMapping()
        {
            ToTable("AatfDeliveryLocation", "PCS");
        }
    }
}
