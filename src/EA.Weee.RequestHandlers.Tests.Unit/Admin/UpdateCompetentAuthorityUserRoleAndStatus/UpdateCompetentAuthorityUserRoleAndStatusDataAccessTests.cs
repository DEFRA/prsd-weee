﻿namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.UpdateCompetentAuthorityUserRoleAndStatus
{
    using DataAccess;
    using Domain.Admin;
    using FakeItEasy;
    using RequestHandlers.Admin.UpdateCompetentAuthorityUserRoleAndStatus;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Weee.Tests.Core;
    using Xunit;

    public class UpdateCompetentAuthorityUserRoleAndStatusDataAccessTests
    {
        private readonly WeeeContext context;
        private readonly DbContextHelper helper;

        public UpdateCompetentAuthorityUserRoleAndStatusDataAccessTests()
        {
            context = A.Fake<WeeeContext>();
            helper = new DbContextHelper();
        }

        [Fact]
        public async Task GetCompetentAuthorityUser_WhenUserDoesExist_ReturnsUser()
        {
            var competentAuthorityUserId = Guid.Empty; // Id cannot be set for existing user so will match

            A.CallTo(() => context.CompetentAuthorityUsers)
                .Returns(helper.GetAsyncEnabledDbSet(new List<CompetentAuthorityUser> { CompetentAuthorityUser() }));

            var result = await UpdateCompetentAuthorityUserStatusDataAccess().GetCompetentAuthorityUser(competentAuthorityUserId);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetCompetentAuthorityUser_WhenUserDoesNotExist_ReturnsNull()
        {
            var competentAuthorityUserId = Guid.NewGuid(); // Id cannot be set for existing user so will not match

            A.CallTo(() => context.CompetentAuthorityUsers)
                .Returns(helper.GetAsyncEnabledDbSet(new List<CompetentAuthorityUser> { CompetentAuthorityUser() }));

            var result = await UpdateCompetentAuthorityUserStatusDataAccess().GetCompetentAuthorityUser(competentAuthorityUserId);

            Assert.Null(result);
        }

        private UpdateCompetentAuthorityUserRoleAndStatusDataAccess UpdateCompetentAuthorityUserStatusDataAccess()
        {
            return new UpdateCompetentAuthorityUserRoleAndStatusDataAccess(context);
        }

        private CompetentAuthorityUser CompetentAuthorityUser()
        {
            return A.Fake<CompetentAuthorityUser>();
        }
    }
}
