namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using EA.Weee.Domain;

    internal class OrganisationUserStatusMapping : ComplexTypeConfiguration<OrganisationUserStatus>
    {
        public OrganisationUserStatusMapping()
        {
            Ignore(x => x.DisplayName);
            Property(x => x.Value).HasColumnName("OrganisationUserStatus");
        }
    }
}
