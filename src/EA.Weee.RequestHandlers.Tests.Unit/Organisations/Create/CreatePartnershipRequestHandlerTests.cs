namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations.Create
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
    using Domain.Organisation;
    using FakeItEasy;
    using Helpers;
    using Prsd.Core.Domain;
    using RequestHandlers.Organisations.Create;
    using Requests.Organisations.Create;
    using Xunit;

    public class CreatePartnershipRequestHandlerTests : CreateOrganisationRequestHandlerTestsBase
    {
        [Fact]
        public async Task NotExternalUser_ThrowsSecurityException()
        {
            var denyingAuthorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            var handler = new CreatePartnershipRequestHandler(denyingAuthorization, A<WeeeContext>._, A<IUserContext>._);

            await Assert.ThrowsAsync<SecurityException>(async () => await handler.HandleAsync(A<CreatePartnershipRequest>._));
        }

        [Fact]
        public async Task HappyPath_CreatesPartnershipAndApprovedOrganisationUser()
        {
            var permissiveAuthorization = new AuthorizationBuilder().AllowExternalAreaAccess().Build();
            
            var context = GetPreparedContext();
            
            var userId = Guid.NewGuid();
            var userContext = A.Fake<IUserContext>();
            A.CallTo(() => userContext.UserId).Returns(userId);

            const string TradingName = "Some trading name";

            var handler = new CreatePartnershipRequestHandler(permissiveAuthorization, context, userContext);

            await handler.HandleAsync(new CreatePartnershipRequest { TradingName = TradingName });

            Assert.NotNull(addedOrganisation);
            Assert.NotNull(addedOrganisationUser);
            Assert.NotEqual(Guid.Empty, addedOrganisationId);

            Assert.Equal(TradingName, addedOrganisation.TradingName);
            Assert.Equal(OrganisationType.Partnership, addedOrganisation.OrganisationType);
            Assert.Equal(addedOrganisationId, addedOrganisationUser.OrganisationId);
            Assert.Equal(UserStatus.Approved, addedOrganisationUser.UserStatus);
        }
    }
}
