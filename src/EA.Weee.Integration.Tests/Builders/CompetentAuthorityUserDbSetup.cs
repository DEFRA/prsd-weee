namespace EA.Weee.Integration.Tests.Builders
{
    using System;
    using Base;
    using Domain.Admin;
    using Domain.User;
    using Weee.Tests.Core;

    public class CompetentAuthorityUserDbSetup : DbTestDataBuilder<CompetentAuthorityUser, CompetentAuthorityUserDbSetup>
    {
        protected override CompetentAuthorityUser Instantiate()
        {
            instance = new CompetentAuthorityUser(DbContext.GetCurrentUser(), Guid.Empty, UserStatus.Active, null);

            return instance;
        }

        public CompetentAuthorityUserDbSetup WithUserIdAndAuthorityAndRole(string userId, Guid authorityId, Guid roleId)
        {
            ObjectInstantiator<CompetentAuthorityUser>.SetProperty(c => c.RoleId, roleId, instance);
            ObjectInstantiator<CompetentAuthorityUser>.SetProperty(c => c.Role, null, instance);
            ObjectInstantiator<CompetentAuthorityUser>.SetProperty(c => c.UserId, userId, instance);
            ObjectInstantiator<CompetentAuthorityUser>.SetProperty(c => c.User, null, instance);
            ObjectInstantiator<CompetentAuthorityUser>.SetProperty(c => c.CompetentAuthorityId, authorityId, instance);
            ObjectInstantiator<CompetentAuthorityUser>.SetProperty(c => c.CompetentAuthority, null, instance);

            return this;
        }
    }
}
