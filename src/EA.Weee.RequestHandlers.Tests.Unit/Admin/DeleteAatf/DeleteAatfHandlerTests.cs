namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.DeleteAatf
{
    using EA.Weee.DataAccess.Identity;
    using EA.Weee.RequestHandlers.AatfReturn.Internal;
    using EA.Weee.RequestHandlers.Admin.DeleteAatf;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin.DeleteAatf;
    using EA.Weee.Security;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using Microsoft.AspNet.Identity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using Xunit;

    public class DeleteAatfHandlerTests
    {
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly IOrganisationDataAccess organisationDataAccess;
        private readonly DeleteAatfHandler handler;

        public DeleteAatfHandlerTests()
        {
            aatfDataAccess = A.Fake<IAatfDataAccess>();
            organisationDataAccess = A.Fake<IOrganisationDataAccess>();

            handler = new DeleteAatfHandler(new AuthorizationBuilder().AllowInternalAreaAccess().Build(),
                aatfDataAccess,
                organisationDataAccess);
        }

        [Theory]
        [Trait("Authorization", "Internal")]
        [InlineData(AuthorizationBuilder.UserType.Unauthenticated)]
        [InlineData(AuthorizationBuilder.UserType.External)]
        public async Task HandleAsync_WithNonInternalAccess_ThrowsSecurityException(AuthorizationBuilder.UserType userType)
        {
            var authorization = AuthorizationBuilder.CreateFromUserType(userType);
            var userManager = A.Fake<UserManager<ApplicationUser>>();

            var handler = new DeleteAatfHandler(authorization, aatfDataAccess, organisationDataAccess);

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
            var handler = new DeleteAatfHandler(authorization, aatfDataAccess, organisationDataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<DeleteAnAatf>());

            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void HandleAsync_DeletesAatfAndOrgIfNoOtherAatfsOnOrg(bool orgHasOtherAatfs)
        {
            var aatfId = Guid.NewGuid();
            var organisationId = Guid.NewGuid();

            A.CallTo(() => aatfDataAccess.DoesAatfOrganisationHaveMoreAatfs(aatfId)).Returns(orgHasOtherAatfs);

            await handler.HandleAsync(new DeleteAnAatf(aatfId, organisationId));

            A.CallTo(() => aatfDataAccess.RemoveAatf(aatfId)).MustHaveHappened(Repeated.Exactly.Once);

            if (!orgHasOtherAatfs)
            {
                A.CallTo(() => organisationDataAccess.Delete(organisationId)).MustHaveHappened(Repeated.Exactly.Once);
            }
            else
            {
                A.CallTo(() => organisationDataAccess.Delete(organisationId)).MustNotHaveHappened();
            }
        }
    }
}
