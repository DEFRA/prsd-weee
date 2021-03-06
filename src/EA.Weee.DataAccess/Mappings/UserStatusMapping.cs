﻿namespace EA.Weee.DataAccess.Mappings
{
    using Domain.User;
    using System.Data.Entity.ModelConfiguration;

    internal class UserStatusMapping : ComplexTypeConfiguration<UserStatus>
    {
        public UserStatusMapping()
        {
            Ignore(x => x.DisplayName);
            Property(x => x.Value).HasColumnName("UserStatus");
        }
    }
}
