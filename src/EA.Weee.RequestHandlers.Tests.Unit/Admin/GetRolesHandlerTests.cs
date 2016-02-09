namespace EA.Weee.RequestHandlers.Tests.Unit.Admin
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Security;
    using DataAccess;
    using FakeItEasy;
    using RequestHandlers.Admin;
    using RequestHandlers.Security;
    using Requests.Admin;
    using Weee.Tests.Core;
    using Xunit;
    using Role = Domain.Security.Role;

    public class GetRolesHandlerTests
    {
        [Fact]
        public async Task HandleAsync_WithNonInternalAccess_ThrowsSecurityException()
        {
            // Arrange
            IWeeeAuthorization authorization = new AuthorizationBuilder()
                .DenyInternalAreaAccess()
                .Build();

            var handler = new GetRolesHandler(authorization, A.Dummy<WeeeContext>());

            // Act
            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<GetRoles>());

            // Assert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task HandleAsync_WithNonInternalAdminRole_ThrowsSecurityException()
        {
            // Arrange
            IWeeeAuthorization authorization = new AuthorizationBuilder()
                .AllowInternalAreaAccess()
                .DenyRole(Roles.InternalAdmin)
                .Build();

            var handler = new GetRolesHandler(authorization, A.Dummy<WeeeContext>());

            // Act
            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<GetRoles>());

            // Assert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task HandleAsync_ReturnsRolesFromDatabase()
        {
            // Arrange
            var role1 = new Role("InternalAdmin", "Administrator");
            var role2 = new Role("InternalUser", "Internal User");
            var role3 = new Role("ExternalUser", "External User");

            var roles = new List<Role> { role1, role2, role3 };
            DbContextHelper dbHelper = new DbContextHelper();

            var weeeContext = A.Fake<WeeeContext>();
            A.CallTo(() => weeeContext.Roles)
                .Returns(dbHelper.GetAsyncEnabledDbSet(roles));

            var handler = new GetRolesHandler(A.Dummy<IWeeeAuthorization>(), weeeContext);

            // Act
            var result = await handler.HandleAsync(new GetRoles());

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Collection(result,
                r1 => Assert.Equal("InternalAdmin", r1.Name),
                r2 => Assert.Equal("InternalUser", r2.Name),
                r3 => Assert.Equal("ExternalUser", r3.Name));
        }
    }
}
