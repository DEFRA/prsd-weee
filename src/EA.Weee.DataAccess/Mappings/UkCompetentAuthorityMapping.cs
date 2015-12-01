namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain;
    internal class UKCompetentAuthorityMapping : EntityTypeConfiguration<UKCompetentAuthority>
    {
        public UKCompetentAuthorityMapping()
        {
            this.ToTable("CompetentAuthority", "Lookup");
        }
    }
}