namespace EA.Weee.DataAccess.Mappings
{
    using EA.Weee.Domain.DataReturns;
    using System.Data.Entity.ModelConfiguration;

    internal class WeeeDeliveredAmountMapping : EntityTypeConfiguration<WeeeDeliveredAmount>
    {
        public WeeeDeliveredAmountMapping()
        {
            ToTable("WeeeDeliveredAmount", "PCS");

            Ignore(ps => ps.IsAatfDeliveredAmount);
            Ignore(ps => ps.IsAeDeliveredAmount);

            Ignore(ps => ps.ObligationType);
            Property(ps => ps.DatabaseObligationType).HasColumnName("ObligationType");

            Property(ps => ps.Tonnage).HasPrecision(28, 3);
        }
    }
}
