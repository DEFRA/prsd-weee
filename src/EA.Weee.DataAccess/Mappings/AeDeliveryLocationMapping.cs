namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.DataReturns;

    internal class AeDeliveryLocationMapping : EntityTypeConfiguration<AeDeliveryLocation>
    {
        public AeDeliveryLocationMapping()
        {
            ToTable("AeDeliveryLocation", "PCS");            
        }
    }
}
