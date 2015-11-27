namespace EA.Weee.RequestHandlers.Tests.Unit.Users.GetManageableOrganisationUsers
{
    using Core.Helpers;
    using DataAccess;
    using Domain;
    using Domain.Organisation;
    using FakeItEasy;
    using Prsd.Core.Domain;
    using RequestHandlers.Users.GetManageableOrganisationUsers;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using Weee.Tests.Core;
    using Xunit;

    public class GetManageableOrganisationUsersDataAccessTests
    {
        private readonly WeeeContext context;

        private readonly DbContextHelper dbHelper;

        public GetManageableOrganisationUsersDataAccessTests()
        {
            context = A.Fake<WeeeContext>();
            dbHelper = new DbContextHelper();
        }

        [Fact]
        public async void ShouldNotReturnUsersWhereOrganisationIdDoesNotMatch()
        {
            var userOrganisationId = Guid.NewGuid();
            var searchOrganisationId = Guid.NewGuid();

            A.CallTo(() => context.OrganisationUsers)
                .Returns(Data(new List<OrganisationUser>
                {
                    new OrganisationUser(Guid.NewGuid(), userOrganisationId, UserStatus.Active)
                }));

            var result = await GetManageableOrganisationUsersDataAccess().GetManageableUsers(searchOrganisationId);

            Assert.Empty(result);
        }

        [Theory]
        [InlineData(Core.Shared.UserStatus.Inactive)]
        [InlineData(Core.Shared.UserStatus.Active)]
        [InlineData(Core.Shared.UserStatus.Pending)]
        [InlineData(Core.Shared.UserStatus.Rejected)]
        public async void SingleOrganisationUserForUser_ShouldGetRequest(Core.Shared.UserStatus userStatus)
        {
            var organisationId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var organisationUser = new OrganisationUser(userId, organisationId,
                userStatus.ToDomainEnumeration<UserStatus>());

            A.CallTo(() => context.OrganisationUsers)
                .Returns(Data(new List<OrganisationUser>
                {
                    organisationUser
                }));

            var result = await GetManageableOrganisationUsersDataAccess().GetManageableUsers(organisationId);

            Assert.Equal(organisationUser, result.Single());
        }

        [Theory]
        [InlineData(Core.Shared.UserStatus.Rejected, Core.Shared.UserStatus.Pending)]
        [InlineData(Core.Shared.UserStatus.Rejected, Core.Shared.UserStatus.Active)]
        [InlineData(Core.Shared.UserStatus.Rejected, Core.Shared.UserStatus.Inactive)]
        public async void MultipleOrganisationUsersForOneUser_ShouldNotRetrieveAnyRejectedOrganisationUsers_WhenNonRejectedOrganisationUserAlsoExists(Core.Shared.UserStatus previousUserStatus, Core.Shared.UserStatus currentUserStatus)
        {
            var organisationId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var previousOrganisationUser = new OrganisationUser(userId, organisationId, previousUserStatus.ToDomainEnumeration<UserStatus>());
            var currentOrganisationUser = new OrganisationUser(userId, organisationId, previousUserStatus.ToDomainEnumeration<UserStatus>());

            A.CallTo(() => context.OrganisationUsers)
                .Returns(Data(new List<OrganisationUser>
                {
                    previousOrganisationUser,
                    currentOrganisationUser
                }));

            var result = await GetManageableOrganisationUsersDataAccess().GetManageableUsers(organisationId);

            Assert.Equal(currentOrganisationUser, result.Single());
        }

        [Fact]
        public async void MultipleOrganisationUsersForOneUser_WithRejectedStatus_RetrievesSingleRejectedOrganisationUser()
        {
            var organisationId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            A.CallTo(() => context.OrganisationUsers)
                .Returns(Data(new List<OrganisationUser>
                {
                    new OrganisationUser(userId, organisationId, UserStatus.Rejected),
                    new OrganisationUser(userId, organisationId, UserStatus.Rejected)
                }));

            var result = await GetManageableOrganisationUsersDataAccess().GetManageableUsers(organisationId);

            Assert.Single(result);
        }

        private DbSet<T> Data<T>(IEnumerable<T> data) where T : Entity
        {
            return dbHelper.GetAsyncEnabledDbSet(data);
        }

        private GetManageableOrganisationUsersDataAccess GetManageableOrganisationUsersDataAccess()
        {
            return new GetManageableOrganisationUsersDataAccess(context);
        }
    }
}
