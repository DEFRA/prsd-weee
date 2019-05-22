namespace EA.Weee.RequestHandlers.Tests.Unit.Users
{
    using EA.Weee.DataAccess.Identity;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.RequestHandlers.Users;
    using EA.Weee.Requests.Users;
    using EA.Weee.Security;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using Microsoft.AspNet.Identity;
    using System.Threading.Tasks;
    using Xunit;

    public class CheckUserInRoleHandlerTests
    {
        [Theory]
        [InlineData(Roles.InternalAdmin, Roles.InternalUser)]
        [InlineData(Roles.InternalUser, Roles.InternalAdmin)]
        public async Task HandleAsync_CheckRoleIsAllowed(Roles allowedRole, Roles notAllowedRole)
        {
            IWeeeAuthorization authorization = new AuthorizationBuilder()
                .AllowInternalAreaAccess()
                .AllowRole(allowedRole)
                .Build();

            CheckUserRoleHandler handler = new CheckUserRoleHandler(authorization);

            bool result = await handler.HandleAsync(new CheckUserRole(allowedRole));

            Assert.Equal(true, result);

            result = await handler.HandleAsync(new CheckUserRole(notAllowedRole));

            Assert.Equal(false, result);
        }
    }
}
