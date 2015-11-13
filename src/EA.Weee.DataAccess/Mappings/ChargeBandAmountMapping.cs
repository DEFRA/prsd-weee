namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;

    internal class ChargeBandAmountMapping : EntityTypeConfiguration<Domain.Lookup.ChargeBandAmount>
    {
        public ChargeBandAmountMapping()
        {
            ToTable("ChargeBandAmount", "Lookup");
        }
    }
}
