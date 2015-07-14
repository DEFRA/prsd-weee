namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Organisation;

    internal class OrganisationUserMapping : EntityTypeConfiguration<OrganisationUser>
    {
        public OrganisationUserMapping()
        {
            ToTable("OrganisationUser", "Organisation");
        }
    }
}