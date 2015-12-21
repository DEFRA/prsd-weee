namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using EA.Weee.Domain.DataReturns;

    internal class WeeeDeliveredAmountMapping : EntityTypeConfiguration<WeeeDeliveredAmount>
    {
        public WeeeDeliveredAmountMapping()
        {
            ToTable("WeeeDeliveredAmount", "PCS");

            Ignore(ps => ps.IsAatfDeliveredAmount);
            Ignore(ps => ps.IsAeDeliveredAmount);

            Ignore(ps => ps.ObligationType);
            Property(ps => ps.DatabaseObligationType).HasColumnName("ObligationType");
        }
    }
}
