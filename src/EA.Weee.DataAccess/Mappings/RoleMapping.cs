namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Security;

    internal class RoleMapping : EntityTypeConfiguration<Role>
    {
        public RoleMapping()
        {
            ToTable("Role", "Security");
        }
    }
}
