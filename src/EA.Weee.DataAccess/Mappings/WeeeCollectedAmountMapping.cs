namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using EA.Weee.Domain.DataReturns;

    internal class WeeeCollectedAmountMapping : EntityTypeConfiguration<WeeeCollectedAmount>
    {
        public WeeeCollectedAmountMapping()
        {
            ToTable("WeeeCollectedAmount", "PCS");

            Ignore(ps => ps.ObligationType);
            Property(ps => ps.DatabaseObligationType).HasColumnName("ObligationType");

            Property(ps => ps.Tonnage).HasPrecision(38, 3);
        }
    }
}
