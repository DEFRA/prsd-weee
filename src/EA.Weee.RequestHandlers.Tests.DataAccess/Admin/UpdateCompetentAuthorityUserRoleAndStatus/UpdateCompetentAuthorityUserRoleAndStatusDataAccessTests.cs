namespace EA.Weee.RequestHandlers.Tests.DataAccess.Admin.UpdateCompetentAuthorityUserRoleAndStatus
{
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Security;
    using Domain.Security;
    using Domain.User;
    using RequestHandlers.Admin.UpdateCompetentAuthorityUserRoleAndStatus;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class UpdateCompetentAuthorityUserRoleAndStatusDataAccessTests
    {
        [Fact]
        public async Task GetRoleOrDefaultAsync_ReturnsNullWhenNoMatchingRoleName()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var dataAccess = new UpdateCompetentAuthorityUserRoleAndStatusDataAccess(databaseWrapper.WeeeContext);

                var result = await dataAccess.GetRoleOrDefaultAsync("Invalid Role name");

                Assert.Null(result);
            }
        }

        [Fact]
        public async Task GetRoleOrDefaultAsync_ReturnsMatchingRoleName()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var dataAccess = new UpdateCompetentAuthorityUserRoleAndStatusDataAccess(databaseWrapper.WeeeContext);

                var result = await dataAccess.GetRoleOrDefaultAsync("InternalAdmin");

                Assert.NotNull(result);
                Assert.Equal("InternalAdmin", result.Name);
            }
        }

        [Fact]
        public async Task UpdateUserRoleAndStatus_UpdatesUserRoleAndStatusInDatabase()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                ModelHelper modelHelper = new ModelHelper(databaseWrapper.Model);
                DomainHelper domainHelper = new DomainHelper(databaseWrapper.WeeeContext);

                var userRole = databaseWrapper.Model.Roles.Single(r => r.Name == "InternalUser");
                var user = modelHelper.GetOrCreateCompetentAuthorityUser("TestUser", 1, userRole);

                databaseWrapper.Model.SaveChanges();

                var competentAuthorityUser = domainHelper.GetCompetentAuthorityUser(user.Id);
                var adminRole = domainHelper.GetRole("InternalAdmin");

                var dataAccess = new UpdateCompetentAuthorityUserRoleAndStatusDataAccess(databaseWrapper.WeeeContext);

                await dataAccess.UpdateUserRoleAndStatus(competentAuthorityUser, adminRole, Core.Shared.UserStatus.Active);

                Assert.Equal(UserStatus.Active, competentAuthorityUser.UserStatus);
                Assert.Equal("InternalAdmin", competentAuthorityUser.Role.Name);
            }
        }
    }
}
