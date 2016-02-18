namespace EA.Weee.RequestHandlers.Tests.DataAccess.Admin
{
    using System.Threading.Tasks;
    using Core.Configuration;
    using FakeItEasy;
    using RequestHandlers.Admin;
    using Requests.Admin;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class AddCompetentAuthorityUserHandlerTests
    {
        [Fact]
        public async Task HandleAsync_CreatesUserWithStandardUserRole()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(database.Model);
                DomainHelper domainHelper = new DomainHelper(database.WeeeContext);

                var user = helper.GetOrCreateUser("TestUser");
                user.Email = "TestUser@environment-agency.gov.uk";

                database.Model.SaveChanges();

                var handler = new AddCompetentAuthorityUserHandler(database.WeeeContext, A.Dummy<ITestUserEmailDomains>());

                // Act
                var competentAuthorityUserId = await handler.HandleAsync(new AddCompetentAuthorityUser(user.Id));
                var competentAuthorityUser = domainHelper.GetCompetentAuthorityUser(competentAuthorityUserId);

                // Assert
                Assert.Equal(user.Id, competentAuthorityUser.UserId);
                Assert.NotNull(competentAuthorityUser.Role);
                Assert.Equal("InternalUser", competentAuthorityUser.Role.Name);
            }
        }
    }
}
