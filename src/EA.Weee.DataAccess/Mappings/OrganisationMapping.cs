namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Organisation;

    internal class OrganisationMapping : EntityTypeConfiguration<Organisation>
    {
        public OrganisationMapping()
        {
            ToTable("Organisation", "Organisation");
        }
    }
}
