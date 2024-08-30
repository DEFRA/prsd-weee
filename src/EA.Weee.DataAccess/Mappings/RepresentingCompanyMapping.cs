namespace EA.Weee.DataAccess.Mappings
{
    using EA.Weee.Domain.Producer;
    using System.Data.Entity.ModelConfiguration;
    using EA.Weee.Domain.Organisation;

    public class RepresentingCompanyConfiguration : EntityTypeConfiguration<RepresentingCompany>
    {
        public RepresentingCompanyConfiguration()
        {
            ToTable("RepresentingCompany", "Organisation");

            HasKey(e => e.Id);

            Property(e => e.RowVersion).IsRowVersion();

            HasRequired(e => e.Country).WithMany().HasForeignKey(e => e.CountryId);
        }
    }
}
