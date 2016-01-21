namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.User;

    internal class UserStatusMapping : ComplexTypeConfiguration<UserStatus>
    {
        public UserStatusMapping()
        {
            Ignore(x => x.DisplayName);
            Property(x => x.Value).HasColumnName("UserStatus");
        }
    }
}
