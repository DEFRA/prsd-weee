namespace EA.Weee.RequestHandlers.Tests.Unit.Admin
{
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.Identity;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.Admin;
    using EA.Weee.RequestHandlers.Organisations;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Security;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using Microsoft.AspNet.Identity;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class CompleteOrganisationAdminHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IOrganisationDetailsDataAccess dataAccess;

        public CompleteOrganisationAdminHandlerTests()
        {
            this.authorization = AuthorizationBuilder.CreateUserWithAllRights();
            this.dataAccess = A.Fake<IOrganisationDetailsDataAccess>();

            DbContextHelper dbContextHelper = new DbContextHelper();
        }

        [Theory]
        [Trait("Authorization", "Internal")]
        [InlineData(AuthorizationBuilder.UserType.Unauthenticated)]
        [InlineData(AuthorizationBuilder.UserType.External)]
        public async Task HandleAsync_WithNonInternalAccess_ThrowsSecurityException(AuthorizationBuilder.UserType userType)
        {
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateFromUserType(userType);
            UserManager<ApplicationUser> userManager = A.Fake<UserManager<ApplicationUser>>();

            CompleteOrganisationAdminHandler handler = new CompleteOrganisationAdminHandler(authorization, this.dataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<CompleteOrganisationAdmin>());

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

            CompleteOrganisationAdminHandler handler = new CompleteOrganisationAdminHandler(authorization, this.dataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<CompleteOrganisationAdmin>());

            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async void HandleAsync_WithValidOrganisationId_SetsStatusToComplete()
        {
            Guid organisationId = Guid.NewGuid();

            Organisation organisation = A.Dummy<Organisation>();
            A.CallTo(() => dataAccess.FetchOrganisationAsync(organisationId))
                .Returns(organisation);

            CompleteOrganisationAdminHandler handler = new CompleteOrganisationAdminHandler(authorization, this.dataAccess);

            CompleteOrganisationAdmin request = new CompleteOrganisationAdmin()
            {
                OrganisationId = organisationId
            };

            var result = await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => dataAccess.FetchOrganisationAsync(organisationId))
                .MustHaveHappened(Repeated.Exactly.Once);

            Assert.Equal(OrganisationStatus.Complete, organisation.OrganisationStatus);

            A.CallTo(() => dataAccess.SaveAsync())
                .MustHaveHappened(Repeated.Exactly.Once);

            Assert.Equal(result, true);
        }

        [Fact]
        public async void HandleAsync_InvalidOrganisationId_ThrowsException()
        {
            Guid organisationId = Guid.NewGuid();
            string errorMessage = $"No organisation was found with an ID of \"{organisationId}\".";

            CompleteOrganisationAdminHandler handler = new CompleteOrganisationAdminHandler(authorization, this.dataAccess);

            CompleteOrganisationAdmin request = new CompleteOrganisationAdmin()
            {
                OrganisationId = organisationId
            };

            A.CallTo(() => dataAccess.FetchOrganisationAsync(organisationId)).Throws(new Exception(errorMessage));

            Func<Task> action = async () => await handler.HandleAsync(request);

            Exception error = await Assert.ThrowsAsync<Exception>(action);

            Assert.Equal(errorMessage, error.Message);
        }
    }
}
