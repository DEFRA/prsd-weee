namespace EA.Weee.Integration.Tests.Builders
{
    using System;
    using Base;
    using Domain.Admin;
    using Domain.User;

    public class CompetentAuthorityUserDbSetup : DbTestDataBuilder<CompetentAuthorityUser, CompetentAuthorityUserDbSetup>
    {
        protected override CompetentAuthorityUser Instantiate()
        {
            instance = new CompetentAuthorityUser(string.Empty, Guid.Empty, UserStatus.Active, null);

            return instance;
        }

        public CompetentAuthorityUserDbSetup WithUserIdAndAuthorityAndRole(string userId, Guid authorityId, Guid roleId)
        {
            instance.UpdateUser(userId, authorityId, roleId);

            return this;
        }
    }
}
