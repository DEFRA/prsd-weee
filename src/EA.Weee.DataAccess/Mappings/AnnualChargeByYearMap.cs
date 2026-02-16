namespace EA.Weee.DataAccess.Mappings
{
    using EA.Weee.Domain.Lookup;
    using System.Data.Entity.ModelConfiguration;
    public class AnnualChargeByYearMap : EntityTypeConfiguration<AnnualChargeByYear>
    {
        public AnnualChargeByYearMap()
        {
            ToTable("AnnualChargeByYear", "Lookup");

            HasKey(a => a.Id);

            Property(a => a.CompetentAuthorityId)
                .IsRequired();

            Property(a => a.ComplianceYear)
                .IsRequired();

            Property(a => a.AnnualChargeAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            Property(a => a.EffectiveFrom)
                .IsOptional();

            HasRequired(a => a.CompetentAuthority)
                .WithMany()
                .HasForeignKey(a => a.CompetentAuthorityId);
        }
    }
}
