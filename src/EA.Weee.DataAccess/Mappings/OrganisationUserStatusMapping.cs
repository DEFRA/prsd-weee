namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain;
 
    internal class OrganisationUserStatusMapping : ComplexTypeConfiguration<UserStatus>
    {
        public OrganisationUserStatusMapping()
        {
            Ignore(x => x.DisplayName);
            Property(x => x.Value).HasColumnName("OrganisationUserStatus");
        }
    }
}
