namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.DataReturns;

    internal class AatfDeliveryLocationMapping : EntityTypeConfiguration<AatfDeliveryLocation>
    {
        public AatfDeliveryLocationMapping()
        {
            ToTable("AatfDeliveryLocation", "PCS");            
        }
    }
}
