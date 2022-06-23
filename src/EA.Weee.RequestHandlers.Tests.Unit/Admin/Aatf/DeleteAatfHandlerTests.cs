namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Aatf
{
    using DataAccess;
    using DataAccess.DataAccess;
    using DataAccess.Identity;
    using FakeItEasy;
    using Microsoft.AspNet.Identity;
    using RequestHandlers.Admin.Aatf;
    using Requests.Admin.Aatf;
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using RequestHandlers.Aatf;
    using Weee.Security;
    using Weee.Tests.Core;
    using Xunit;

    public class DeleteAatfHandlerTests
    {
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly IOrganisationDataAccess organisationDataAccess;
        private readonly WeeeContext weeeContext;
        private readonly IGetAatfDeletionStatus getAatfDeletionStatus;

        public DeleteAatfHandlerTests()
        {
            aatfDataAccess = A.Fake<IAatfDataAccess>();
            organisationDataAccess = A.Fake<IOrganisationDataAccess>();
            weeeContext = A.Fake<WeeeContext>();
            getAatfDeletionStatus = A.Fake<IGetAatfDeletionStatus>();
        }

        [Theory]
        [Trait("Authorization", "Internal")]
        [InlineData(AuthorizationBuilder.UserType.Unauthenticated)]
        [InlineData(AuthorizationBuilder.UserType.External)]
        public async Task HandleAsync_WithNonInternalAccess_ThrowsSecurityException(AuthorizationBuilder.UserType userType)
        {
            var authorization = AuthorizationBuilder.CreateFromUserType(userType);
            var userManager = A.Fake<UserManager<ApplicationUser>>();

            var handler = new DeleteAatfHandler(authorization,
                aatfDataAccess,
                organisationDataAccess,
                weeeContext,
                getAatfDeletionStatus);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<DeleteAnAatf>());

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
            var handler = new DeleteAatfHandler(authorization,
                aatfDataAccess,
                organisationDataAccess,
                weeeContext,
                getAatfDeletionStatus);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<DeleteAnAatf>());

            await Assert.ThrowsAsync<SecurityException>(action);
        }
    }
}
