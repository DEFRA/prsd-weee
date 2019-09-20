namespace EA.Weee.DataAccess.Mappings
{
    using Domain.Security;
    using System.Data.Entity.ModelConfiguration;

    internal class RoleMapping : EntityTypeConfiguration<Role>
    {
        public RoleMapping()
        {
            ToTable("Role", "Security");
        }
    }
}
