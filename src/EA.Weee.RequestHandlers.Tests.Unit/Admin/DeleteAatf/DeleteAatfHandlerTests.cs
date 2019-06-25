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
    using Xunit;

    public class DeleteAatfHandlerTests
    {
        private readonly IAatfDataAccess dataAccess;
        private readonly IWeeeAuthorization authorization;

        public DeleteAatfHandlerTests()
        {
            this.authorization = A.Fake<IWeeeAuthorization>();
            this.dataAccess = A.Fake<IAatfDataAccess>();
        }

        [Theory]
        [Trait("Authorization", "Internal")]
        [InlineData(AuthorizationBuilder.UserType.Unauthenticated)]
        [InlineData(AuthorizationBuilder.UserType.External)]
        public async Task HandleAsync_WithNonInternalAccess_ThrowsSecurityException(AuthorizationBuilder.UserType userType)
        {
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateFromUserType(userType);
            UserManager<ApplicationUser> userManager = A.Fake<UserManager<ApplicationUser>>();

            DeleteAatfHandler handler = new DeleteAatfHandler(authorization, dataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<DeleteAnAatf>());

            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task HandleAsync_WithNonInternalAdminRole_ThrowsSecurityException()
        {
            IWeeeAuthorization authorization = new AuthorizationBuilder()
                .AllowInternalAreaAccess()
                .DenyRole(Roles.InternalAdmin)
                .Build();

            UserManager<ApplicationUser> userManager = A.Fake<UserManager<ApplicationUser>>();

            DeleteAatfHandler handler = new DeleteAatfHandler(authorization, dataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<DeleteAnAatf>());

            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void HandleAsync_DeletesAatfAndOrgIfNoOtherAatfsOnOrg(bool orgHasOtherAatfs)
        {
            Guid aatfId = Guid.NewGuid();
            Guid organisationId = Guid.NewGuid();

            A.CallTo(() => dataAccess.DoesAatfOrganisationHaveMoreAatfs(aatfId)).Returns(orgHasOtherAatfs);

            DeleteAatfHandler handler = new DeleteAatfHandler(authorization, dataAccess);

            await handler.HandleAsync(new DeleteAnAatf(aatfId, organisationId));

            A.CallTo(() => dataAccess.DeleteAatf(aatfId)).MustHaveHappened(Repeated.Exactly.Once);

            if (!orgHasOtherAatfs)
            {
                A.CallTo(() => dataAccess.DeleteOrganisation(organisationId)).MustHaveHappened(Repeated.Exactly.Once);
            }
            else
            {
                A.CallTo(() => dataAccess.DeleteOrganisation(organisationId)).MustNotHaveHappened();
            }
        }
    }
}
