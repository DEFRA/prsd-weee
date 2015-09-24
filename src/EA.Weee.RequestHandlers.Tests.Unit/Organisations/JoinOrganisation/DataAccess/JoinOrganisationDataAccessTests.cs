namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations.JoinOrganisation.DataAccess
{
    using System;
    using System.Collections.Generic;
    using Core.Helpers;
    using Domain;
    using Domain.Organisation;
    using FakeItEasy;
    using RequestHandlers.Organisations.JoinOrganisation.DataAccess;
    using Weee.DataAccess;
    using Weee.Tests.Core;
    using Xunit;

    public class JoinOrganisationDataAccessTests
    {
        private readonly WeeeContext context;
        private readonly DbContextHelper contextHelper;

        public JoinOrganisationDataAccessTests()
        {
            context = A.Fake<WeeeContext>();
            contextHelper = new DbContextHelper();

            // By default, context returns empty list for each dbset used in these tests
            A.CallTo(() => context.OrganisationUsers)
                .Returns(contextHelper.GetAsyncEnabledDbSet(new List<OrganisationUser>()));

            A.CallTo(() => context.Organisations)
                .Returns(contextHelper.GetAsyncEnabledDbSet(new List<Organisation>()));

            A.CallTo(() => context.Users)
                .Returns(contextHelper.GetAsyncEnabledDbSet(new List<User>()));
        }

        [Fact]
        public async void DoesUserExist_WhenUserExists_ReturnsTrue()
        {
            A.CallTo(() => context.Users)
                .Returns(contextHelper.GetAsyncEnabledDbSet(new List<User>
                {
                    ValidUser()
                }));

            // Our test user has an empty guid too, so will match
            Assert.True(await JoinOrganisationDataAccess().DoesUserExist(Guid.Empty));
        }

        [Fact]
        public async void DoesUserExist_WhenUserDoesNotExist_ReturnsFalse()
        {
            A.CallTo(() => context.Users)
                .Returns(contextHelper.GetAsyncEnabledDbSet(new List<User>
                {
                    ValidUser()
                }));

            Assert.False(await JoinOrganisationDataAccess().DoesUserExist(Guid.NewGuid()));
        }

        [Fact]
        public async void DoesOrganisationExist_WhenOrganisationExists_ReturnsTrue()
        {
            A.CallTo(() => context.Organisations)
                .Returns(contextHelper.GetAsyncEnabledDbSet(new List<Organisation>
                {
                    ValidOrganisation()
                }));

            // Our test organisation has an empty guid too, so will match
            Assert.True(await JoinOrganisationDataAccess().DoesOrganisationExist(Guid.Empty));
        }

        [Fact]
        public async void DoesOrganisationExist_WhenOrganisationDoesNotExist_ReturnsFalse()
        {
            A.CallTo(() => context.Organisations)
                .Returns(contextHelper.GetAsyncEnabledDbSet(new List<Organisation>
                {
                    ValidOrganisation()
                }));

            Assert.False(await JoinOrganisationDataAccess().DoesOrganisationExist(Guid.NewGuid()));
        }

        [Theory]
        [InlineData(Core.Shared.UserStatus.Active)]
        [InlineData(Core.Shared.UserStatus.Pending)]
        [InlineData(Core.Shared.UserStatus.Inactive)]
        public async void JoinOrganisation_OrganisationUserExistsWithNonRejectedStatus_ReturnsFailure_AndDoesNotSaveAnyChanges(Core.Shared.UserStatus userStatus)
        {
            var organisationId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            A.CallTo(() => context.OrganisationUsers)
                .Returns(contextHelper.GetAsyncEnabledDbSet(new List<OrganisationUser>
                {
                    ValidOrganisationUser(userId, organisationId, userStatus)
                }));

            var result =
                await JoinOrganisationDataAccess()
                    .JoinOrganisation(new OrganisationUser(userId, organisationId, UserStatus.Pending));

            Assert.False((result).Successful);
            
            A.CallTo(() => context.SaveChangesAsync())
                .MustNotHaveHappened();
        }

        [Fact]
        public async void JoinOrganisation_OrganisationUserExistsWithRejectedStatus_SavesChanges_AndReturnsSuccess()
        {
            var organisationId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var organisationUser = new OrganisationUser(userId, organisationId, UserStatus.Pending);

            A.CallTo(() => context.OrganisationUsers)
                .Returns(contextHelper.GetAsyncEnabledDbSet(new List<OrganisationUser>
                {
                    ValidOrganisationUser(userId, organisationId, Core.Shared.UserStatus.Rejected)
                }));

            var result =
                await JoinOrganisationDataAccess()
                    .JoinOrganisation(organisationUser);
            
            A.CallTo(() => context.SaveChangesAsync())
                .MustHaveHappened(Repeated.Exactly.Once);

            Assert.True(result.Successful);
        }

        [Fact]
        public async void JoinOrganisation_OrganisationUserDoesNotExist_SavesChanges_AndReturnsSuccess()
        {
            var organisationId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var organisationUser = new OrganisationUser(userId, organisationId, UserStatus.Pending);

            var result =
                await JoinOrganisationDataAccess()
                    .JoinOrganisation(organisationUser);

            A.CallTo(() => context.SaveChangesAsync())
                .MustHaveHappened(Repeated.Exactly.Once);

            Assert.True(result.Successful);
        }

        private JoinOrganisationDataAccess JoinOrganisationDataAccess()
        {
            return new JoinOrganisationDataAccess(context);
        }

        private User ValidUser()
        {
            return new User(Guid.Empty.ToString(), "first name", "second name", "a@b.c");
        }

        private Organisation ValidOrganisation()
        {
            return Organisation.CreateSoleTrader("trading name");
        }

        private OrganisationUser ValidOrganisationUser(Guid userId, Guid organisationId, Core.Shared.UserStatus userStatus)
        {
            return new OrganisationUser(userId, organisationId, userStatus.ToDomainEnumeration<UserStatus>());
        }
    }
}
