namespace EA.Weee.DataAccess.Mappings
{
    using Domain.DataReturns;
    using System.Data.Entity.ModelConfiguration;

    internal class AeDeliveryLocationMapping : EntityTypeConfiguration<AeDeliveryLocation>
    {
        public AeDeliveryLocationMapping()
        {
            ToTable("AeDeliveryLocation", "PCS");
        }
    }
}
