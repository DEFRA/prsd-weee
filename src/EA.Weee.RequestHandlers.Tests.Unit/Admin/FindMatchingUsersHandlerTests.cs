namespace EA.Weee.RequestHandlers.Tests.Unit.Admin
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using Core.Admin;
    using DataAccess;
    using Domain;
    using Domain.Admin;
    using Domain.Organisation;
    using FakeItEasy;
    using Helpers;
    using RequestHandlers.Admin;
    using Requests.Admin;
    using Requests.Shared;
    using Xunit;

    public class FindMatchingUsersHandlerTests
    {
        private readonly DbContextHelper helper = new DbContextHelper();
        
        public DbSet<User> UsersDbSet { get; set; }
        public DbSet<Organisation> OrganisationDbSet { get; set; }
        public DbSet<OrganisationUser> OrganisationUsersDbSet { get; set; }
        public DbSet<UKCompetentAuthority> UKCompetentAuthoritiesDbSet { get; set; }
        public DbSet<CompetentAuthorityUser> CompetentAuthorityUsersDbSet { get; set; }

        public static Guid UserId = Guid.NewGuid();
        public static Organisation FakeOrganisation;
        private readonly WeeeContext context;
        private readonly FindMatchingUsersHandler handler;
        private static readonly FindMatchingUsers Message = new FindMatchingUsers();
        private readonly OrganisationHelper orgHelper = new OrganisationHelper();
        
        public FindMatchingUsersHandlerTests()
        {
            UsersDbSet = A.Fake<DbSet<User>>();
            OrganisationDbSet = A.Fake<DbSet<Organisation>>();

            var users = new[]
            {
                FakeUserData()
            };

            UsersDbSet = helper.GetAsyncEnabledDbSet(users);

            var organisations = new[]
            {
                FakeOrganisationData()
            };

            OrganisationDbSet = helper.GetAsyncEnabledDbSet(organisations);
            FakeOrganisation = organisations[0];
            var organisationUsers = new[]
            {
                FakeOrganisationUserData()
            };

            OrganisationUsersDbSet = helper.GetAsyncEnabledDbSet(organisationUsers);

            UKCompetentAuthoritiesDbSet = A.Fake<DbSet<UKCompetentAuthority>>();
            CompetentAuthorityUsersDbSet = A.Fake<DbSet<CompetentAuthorityUser>>();

            var competentAuthorites = new[]
            {
                FakeCompetentAuthorityData()
            };

            UKCompetentAuthoritiesDbSet = helper.GetAsyncEnabledDbSet(competentAuthorites);

            var competentAuthorityUsers = new[]
            {
                FakeCompetentAuthorityUser()
            };

            CompetentAuthorityUsersDbSet = helper.GetAsyncEnabledDbSet(competentAuthorityUsers);

            context = A.Fake<WeeeContext>();
            
            A.CallTo(() => context.Users).Returns(UsersDbSet);
            A.CallTo(() => context.Organisations).Returns(OrganisationDbSet);
            A.CallTo(() => context.OrganisationUsers).Returns(OrganisationUsersDbSet);
            A.CallTo(() => context.UKCompetentAuthorities).Returns(UKCompetentAuthoritiesDbSet);
            A.CallTo(() => context.CompetentAuthorityUsers).Returns(CompetentAuthorityUsersDbSet);

            handler = new FindMatchingUsersHandler(context);
        }

        [Fact]
        public async void FindMatchingUsersHandler_ReturnsAllUsers()
        {
            UserSearchDataResult users = await handler.HandleAsync(Message);
            Assert.NotEmpty(users.Results);
        }

        private static User FakeUserData()
        {
            return new User(UserId.ToString(), "FirstName", "Surname", "test@co.uk");
        }

        private Organisation FakeOrganisationData()
        {
            Organisation organisation = orgHelper.GetOrganisationWithDetails("Test", "TestTradingName", "12345678",
                OrganisationType.SoleTraderOrIndividual, OrganisationStatus.Complete);
            return organisation;
        }

        private CompetentAuthorityUser FakeCompetentAuthorityUser()
        {
            CompetentAuthorityUser user = new CompetentAuthorityUser(UserId.ToString(), new Guid(), UserStatus.Pending);
            return user;
        }

        private UKCompetentAuthority FakeCompetentAuthorityData()
        {
            UKCompetentAuthority competentAuthority = new UKCompetentAuthority("Environment Agency", "EA", new Country(new Guid(), "UK - England"));
            return competentAuthority;
        }

        private OrganisationUser FakeOrganisationUserData()
        {
            OrganisationUser orgUser = new OrganisationUser(UserId, FakeOrganisation.Id, UserStatus.Approved);
            return orgUser;
        }
    }
}
