namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;

    internal class OnlineMarketplaceChargeMapping : EntityTypeConfiguration<Domain.Lookup.OnlineMarketplaceCharge>
    {
        public OnlineMarketplaceChargeMapping()
        {
            ToTable("OnlineMarketplaceCharge", "Lookup");
        }
    }
}