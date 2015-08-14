namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Admin;

    public class CompetentAuthorityUserMapping : EntityTypeConfiguration<CompetentAuthorityUser>
    {
        public CompetentAuthorityUserMapping()
        {
            ToTable("CompetentAuthorityUser", "Admin");
        }
    }
}
