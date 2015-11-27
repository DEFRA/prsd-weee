namespace EA.Weee.RequestHandlers.Tests.Unit.Users.GetManageableOrganisationUsers
{
    using Core.Organisations;
    using Domain;
    using Domain.Organisation;
    using FakeItEasy;
    using Prsd.Core.Mapper;
    using RequestHandlers.Security;
    using RequestHandlers.Users.GetManageableOrganisationUsers;
    using Requests.Users.GetManageableOrganisationUsers;
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;

    public class GetUsersByOrganisationIdHandlerTests
    {
        private readonly IGetManageableOrganisationUsersDataAccess dataAccess;
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IMap<OrganisationUser, OrganisationUserData> mapper;

        public GetUsersByOrganisationIdHandlerTests()
        {
            dataAccess = A.Fake<IGetManageableOrganisationUsersDataAccess>();
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            mapper = A.Fake<IMap<OrganisationUser, OrganisationUserData>>();
        }

        [Fact]
        public async void ShouldCheckUserIsAbleToAccessOrganisation()
        {
            var organisationId = Guid.NewGuid();

            await GetManageableOrganisationUsersHandler().HandleAsync(new GetManageableOrganisationUsers(organisationId));

            A.CallTo(() => weeeAuthorization.EnsureOrganisationAccess(organisationId))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void UserIsUnableToAccessOrganisation_ShouldNotGetManageableOrganisationUsers()
        {
            var organisationId = Guid.NewGuid();

            A.CallTo(() => weeeAuthorization.EnsureOrganisationAccess(organisationId))
                .Throws<SecurityException>();

            await Assert.ThrowsAsync<SecurityException>(
                () =>
                    GetManageableOrganisationUsersHandler()
                        .HandleAsync(new GetManageableOrganisationUsers(organisationId)));

            A.CallTo(() => dataAccess.GetManageableUsers(A<Guid>._))
                .MustNotHaveHappened();
        }

        [Fact]
        public async void UserIsAbleToAccessOrganisation_ShouldGetManageableUsers() // By default, an exception is not thrown on authorization check
        {
            var organisationId = Guid.NewGuid();

            await GetManageableOrganisationUsersHandler().HandleAsync(new GetManageableOrganisationUsers(organisationId));

            A.CallTo(() => dataAccess.GetManageableUsers(organisationId))
                .MustHaveHappened();
        }

        [Fact]
        public async Task GetManageableUsers_ShouldMapOrganisationUsersToOrganisationUserData()
        {
            var noOfUsers = 5; // Should map this number of users

            var organisationId = Guid.NewGuid();

            A.CallTo(() => dataAccess.GetManageableUsers(organisationId))
                .Returns(OrganisationUsers(noOfUsers, organisationId));

            var result =
                await
                    GetManageableOrganisationUsersHandler()
                        .HandleAsync(new GetManageableOrganisationUsers(organisationId));

            A.CallTo(() => mapper.Map(A<OrganisationUser>._))
                .MustHaveHappened(Repeated.Exactly.Times(noOfUsers));

            Assert.Equal(noOfUsers, result.Count);
        }

        private IEnumerable<OrganisationUser> OrganisationUsers(int quantity, Guid organisationId)
        {
            for (var i = 0; i < quantity; i++)
            {
                yield return new OrganisationUser(Guid.NewGuid(), organisationId, UserStatus.Active);
            }
        }

        private GetManageableOrganisationUsersHandler GetManageableOrganisationUsersHandler()
        {
            return new GetManageableOrganisationUsersHandler(dataAccess, weeeAuthorization, mapper);
        }
    }
}
