namespace EA.Weee.DataAccess.Mappings
{
    using Domain.Organisation;
    using System.Data.Entity.ModelConfiguration;

    internal class OrganisationUserMapping : EntityTypeConfiguration<OrganisationUser>
    {
        public OrganisationUserMapping()
        {
            ToTable("OrganisationUser", "Organisation");
        }
    }
}