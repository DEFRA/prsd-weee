namespace EA.Weee.DataAccess.Mappings
{
    using Domain.Admin;
    using System.Data.Entity.ModelConfiguration;

    public class CompetentAuthorityUserMapping : EntityTypeConfiguration<CompetentAuthorityUser>
    {
        public CompetentAuthorityUserMapping()
        {
            ToTable("CompetentAuthorityUser", "Admin");
        }
    }
}
