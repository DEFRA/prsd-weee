namespace EA.Weee.RequestHandlers.Tests.Unit.Admin
{
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.Identity;
    using EA.Weee.RequestHandlers.Aatf;
    using EA.Weee.RequestHandlers.Admin.RemoveAATFOrAeRecords;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin.RemoveAATFOrAeRecords;
    using EA.Weee.Security;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using Microsoft.AspNet.Identity;
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;

    public class DeleteAatfOrAeByIdHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly WeeeContext weeeContext;

        private readonly DeleteAatfOrAeByIdHandler handler;

        public DeleteAatfOrAeByIdHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            aatfDataAccess = A.Fake<IAatfDataAccess>();
            weeeContext = A.Fake<WeeeContext>();

            handler = new DeleteAatfOrAeByIdHandler(
                authorization,
                aatfDataAccess,
                weeeContext);
        }

        [Theory]
        [Trait("Authorization", "Internal")]
        [InlineData(AuthorizationBuilder.UserType.Unauthenticated)]
        [InlineData(AuthorizationBuilder.UserType.External)]
        public async Task HandleAsync_WithNonInternalAccess_ThrowsSecurityException(AuthorizationBuilder.UserType userType)
        {
            var authorization = AuthorizationBuilder.CreateFromUserType(userType);
            var userManager = A.Fake<UserManager<ApplicationUser>>();

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<DeleteAatfOrAeRecordById>());

            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task HandleAsync_WithNonInternalAdminRole_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder()
                .AllowInternalAreaAccess()
                .DenyRole(Roles.InternalAdmin)
                .Build();

            var userManager = A.Fake<UserManager<ApplicationUser>>();

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<DeleteAatfOrAeRecordById>());

            await Assert.ThrowsAsync<SecurityException>(action);
        }
    }
}
