namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.DataReturns;

    public class EeeOutputAmountMapping : EntityTypeConfiguration<EeeOutputAmount>
    {
        public EeeOutputAmountMapping()
        {
            ToTable("EeeOutputAmount", "PCS");

            Ignore(ps => ps.ObligationType);
            Property(ps => ps.DatabaseObligationType).HasColumnName("ObligationType");

            Property(ps => ps.Tonnage).HasPrecision(38, 3);
        }
    }
}
