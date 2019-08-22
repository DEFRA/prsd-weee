namespace EA.Weee.DataAccess.Mappings
{
    using Domain.DataReturns;
    using System.Data.Entity.ModelConfiguration;

    public class EeeOutputAmountMapping : EntityTypeConfiguration<EeeOutputAmount>
    {
        public EeeOutputAmountMapping()
        {
            ToTable("EeeOutputAmount", "PCS");

            Ignore(ps => ps.ObligationType);
            Property(ps => ps.DatabaseObligationType).HasColumnName("ObligationType");

            Property(ps => ps.Tonnage).HasPrecision(28, 3);
        }
    }
}
