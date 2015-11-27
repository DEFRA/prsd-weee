namespace EA.Weee.DataAccess.Mappings
{
    using Domain;
    using System.Data.Entity.ModelConfiguration;
    internal class UKCompetentAuthorityMapping : EntityTypeConfiguration<UKCompetentAuthority>
    {
        public UKCompetentAuthorityMapping()
        {
            this.ToTable("CompetentAuthority", "Lookup");
        }
    }
}