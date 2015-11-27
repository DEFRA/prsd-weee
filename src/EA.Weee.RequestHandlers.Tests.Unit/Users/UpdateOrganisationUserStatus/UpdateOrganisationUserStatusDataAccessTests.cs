namespace EA.Weee.RequestHandlers.Tests.Unit.Users.UpdateOrganisationUserStatus
{
    using System;
    using System.Collections.Generic;
    using DataAccess;
    using Domain.Organisation;
    using FakeItEasy;
    using RequestHandlers.Users.UpdateOrganisationUserStatus;
    using Weee.Tests.Core;
    using Xunit;

    public class UpdateOrganisationUserStatusDataAccessTests
    {
        private readonly WeeeContext context;
        private readonly DbContextHelper helper;

        public UpdateOrganisationUserStatusDataAccessTests()
        {
            context = A.Fake<WeeeContext>();
            helper = new DbContextHelper();
        }

        [Fact]
        public async void GetOrganisationUser_WhenOrganisationUserDoesNotExist_ReturnsNull()
        {
            var organisationUserId = Guid.Empty; // Id cannot be set for existing user so will match

            A.CallTo(() => context.OrganisationUsers)
                .Returns(helper.GetAsyncEnabledDbSet(new List<OrganisationUser> { OrganisationUser() }));

            var result = await UpdateOrganisationUserStatusDataAccess().GetOrganisationUser(organisationUserId);

            Assert.NotNull(result);
        }

        [Fact]
        public async void GetOrganisationUser_WhenOrganisationUserDoesExist_ReturnsUser()
        {
            var organisationUserId = Guid.NewGuid(); // Id cannot be set for existing user so will not match

            A.CallTo(() => context.OrganisationUsers)
                .Returns(helper.GetAsyncEnabledDbSet(new List<OrganisationUser> { OrganisationUser() }));

            var result = await UpdateOrganisationUserStatusDataAccess().GetOrganisationUser(organisationUserId);

            Assert.Null(result);
        }

        private UpdateOrganisationUserStatusDataAccess UpdateOrganisationUserStatusDataAccess()
        {
            return new UpdateOrganisationUserStatusDataAccess(context);
        }

        private OrganisationUser OrganisationUser()
        {
            return A.Fake<OrganisationUser>();
        }
    }
}
