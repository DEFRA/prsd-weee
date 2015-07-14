namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Organisation;

    internal class OrganisationUserStatusMapping : ComplexTypeConfiguration<OrganisationUserStatus>
    {
        public OrganisationUserStatusMapping()
        {
            Ignore(x => x.DisplayName);
            Property(x => x.Value).HasColumnName("OrganisationUserStatus");
        }
    }
}
