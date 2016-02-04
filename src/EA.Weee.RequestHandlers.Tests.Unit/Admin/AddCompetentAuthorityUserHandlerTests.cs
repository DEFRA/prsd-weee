namespace EA.Weee.RequestHandlers.Tests.Unit.Admin
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using Core.Configuration;
    using DataAccess;
    using Domain;
    using Domain.Admin;
    using Domain.Security;
    using Domain.User;
    using FakeItEasy;
    using RequestHandlers.Admin;
    using Requests.Admin;
    using Weee.Tests.Core;
    using Xunit;

    public class AddCompetentAuthorityUserHandlerTests
    {
        private readonly DbContextHelper helper = new DbContextHelper();

        public DbSet<User> UsersDbSet { get; set; }
        public DbSet<UKCompetentAuthority> UKCompetentAuthoritiesDbSet { get; set; }
        public DbSet<CompetentAuthorityUser> CompetentAuthorityUsersDbSet { get; set; }

        public static Guid InternalUserId = Guid.NewGuid();
        public static Guid FakeUserId = Guid.NewGuid();
        private readonly WeeeContext context;
        private readonly AddCompetentAuthorityUserHandler handler;
        private readonly IConfigurationManagerWrapper configurationManagerWrapper;
        private readonly ITestUserEmailDomains testInternalUserEmailDomains;

        public AddCompetentAuthorityUserHandlerTests()
        {
            UsersDbSet = A.Fake<DbSet<User>>();

            var users = new[]
            {
                FakeUserData(),
                FakeInternalUserData()
            };

            UsersDbSet = helper.GetAsyncEnabledDbSet(users);

            UKCompetentAuthoritiesDbSet = A.Fake<DbSet<UKCompetentAuthority>>();

            var competentAuthorites = new[]
            {
                FakeCompetentAuthorityData()
            };

            UKCompetentAuthoritiesDbSet = helper.GetAsyncEnabledDbSet(competentAuthorites);

            var roles = new[]
            {
                new Role(RoleNames.InternalUser, "Standard user")
            };

            configurationManagerWrapper = A.Fake<IConfigurationManagerWrapper>();

            context = A.Fake<WeeeContext>();

            A.CallTo(() => context.Users).Returns(UsersDbSet);
            A.CallTo(() => context.UKCompetentAuthorities).Returns(UKCompetentAuthoritiesDbSet);
            A.CallTo(() => context.Roles).Returns(helper.GetAsyncEnabledDbSet(roles));

            testInternalUserEmailDomains = A.Fake<ITestUserEmailDomains>();

            handler = new AddCompetentAuthorityUserHandler(context, testInternalUserEmailDomains);
        }

        [Fact]
        public async void AddCompetentAuthorityUserHandler_ReturnsCompetentAuthorityId()
        {
            AddCompetentAuthorityUser message = new AddCompetentAuthorityUser(InternalUserId.ToString());
            var id = await handler.HandleAsync(message);
            A.CallTo(() => context.SaveChangesAsync()).MustHaveHappened();
            Assert.NotNull(id);
        }

        [Fact]
        public async void AddCompetentAuthorityUserHandler_FakeUser_ThrowsException()
        {
            AddCompetentAuthorityUser message = new AddCompetentAuthorityUser(FakeUserId.ToString());
            A.CallTo(() => configurationManagerWrapper.HasKey("Weee.InternalUsersTestMode")).Returns(true);
            A.CallTo(() => configurationManagerWrapper.GetKeyValue("Weee.InternalUsersTestMode")).Returns("false");
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await handler.HandleAsync(message));
        }

        [Fact]
        public async void AddCompetentAuthorityUserHandler_InternalUsersModeSet_ReturnsSucess()
        {
            A.CallTo(() => testInternalUserEmailDomains.UserTestModeEnabled).Returns(true);
            A.CallTo(() => testInternalUserEmailDomains.Domains).Returns(new List<string>() { "co.uk" });

            AddCompetentAuthorityUser message = new AddCompetentAuthorityUser(FakeUserId.ToString());
            A.CallTo(() => configurationManagerWrapper.HasKey("Weee.InternalUsersTestMode")).Returns(true);
            A.CallTo(() => configurationManagerWrapper.GetKeyValue("Weee.InternalUsersTestMode")).Returns("true");
            Guid id = await handler.HandleAsync(message);
            A.CallTo(() => context.SaveChangesAsync()).MustHaveHappened();
            Assert.NotNull(id);
        }

        private static User FakeUserData()
        {
            return new User(FakeUserId.ToString(), "FirstName", "Surname", "test@co.uk");
        }

        private static User FakeInternalUserData()
        {
            return new User(InternalUserId.ToString(), "FirstName", "Surname", "test@environment-agency.gov.uk");
        }

        private UKCompetentAuthority FakeCompetentAuthorityData()
        {
            UKCompetentAuthority competentAuthority = new UKCompetentAuthority(Guid.NewGuid(), "Environment Agency", "EA", new Country(Guid.NewGuid(), "UK - England"));
            return competentAuthority;
        }
    }
}