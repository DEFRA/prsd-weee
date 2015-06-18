namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using EA.Weee.Domain;

    internal class OrganisationUserMapping : EntityTypeConfiguration<OrganisationUser>
    {
        public OrganisationUserMapping()
        {
            ToTable("OrganisationUser", "Organisation");
        }
    }
}