namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.UpdateCompetentAuthorityUserStatus
{
    using DataAccess;
    using Domain.Admin;
    using FakeItEasy;
    using RequestHandlers.Admin.UpdateCompetentAuthorityUserStatus;
    using System;
    using System.Collections.Generic;
    using Weee.Tests.Core;
    using Xunit;

    public class UpdateCompetentAuthorityUserStatusDataAccessTests
    {
        private readonly WeeeContext context;
        private readonly DbContextHelper helper;

        public UpdateCompetentAuthorityUserStatusDataAccessTests()
        {
            context = A.Fake<WeeeContext>();
            helper = new DbContextHelper();
        }

        [Fact]
        public async void GetCompetentAuthorityUser_WhenUserDoesExist_ReturnsUser()
        {
            var competentAuthorityUserId = Guid.Empty; // Id cannot be set for existing user so will match

            A.CallTo(() => context.CompetentAuthorityUsers)
                .Returns(helper.GetAsyncEnabledDbSet(new List<CompetentAuthorityUser> { CompetentAuthorityUser() }));

            var result = await UpdateCompetentAuthorityUserStatusDataAccess().GetCompetentAuthorityUser(competentAuthorityUserId);

            Assert.NotNull(result);
        }

        [Fact]
        public async void GetCompetentAuthorityUser_WhenUserDoesNotExist_ReturnsNull()
        {
            var competentAuthorityUserId = Guid.NewGuid(); // Id cannot be set for existing user so will not match

            A.CallTo(() => context.CompetentAuthorityUsers)
                .Returns(helper.GetAsyncEnabledDbSet(new List<CompetentAuthorityUser> { CompetentAuthorityUser() }));

            var result = await UpdateCompetentAuthorityUserStatusDataAccess().GetCompetentAuthorityUser(competentAuthorityUserId);

            Assert.Null(result);
        }

        private UpdateCompetentAuthorityUserStatusDataAccess UpdateCompetentAuthorityUserStatusDataAccess()
        {
            return new UpdateCompetentAuthorityUserStatusDataAccess(context);
        }

        private CompetentAuthorityUser CompetentAuthorityUser()
        {
            return A.Fake<CompetentAuthorityUser>();
        }
    }
}
