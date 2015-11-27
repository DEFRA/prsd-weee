namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations.Create
{
    using DataAccess;
    using Domain.Organisation;
    using FakeItEasy;
    using Prsd.Core.Domain;
    using RequestHandlers.Organisations.Create;
    using Requests.Organisations.Create;
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;

    public class CreatePartnershipRequestHandlerTests : CreateOrganisationRequestHandlerTestsBase
    {
        [Fact]
        public async Task CreatePartnershipRequestHandler_NotExternalUser_ThrowsSecurityException()
        {
            var handler = new CreatePartnershipRequestHandler(denyingAuthorization, A<WeeeContext>._, A<IUserContext>._);

            await Assert.ThrowsAsync<SecurityException>(async () => await handler.HandleAsync(A<CreatePartnershipRequest>._));
        }

        [Fact]
        public async Task CreatePartnershipRequestHandler_HappyPath_CreatesPartnershipAndApprovedOrganisationUser()
        {
            var context = GetPreparedContext();
            var userContext = GetPreparedUserContext();

            const string TradingName = "Some trading name";

            var handler = new CreatePartnershipRequestHandler(permissiveAuthorization, context, userContext);
            await handler.HandleAsync(new CreatePartnershipRequest { TradingName = TradingName });

            DoSharedAssertions(TradingName);
            Assert.Equal(OrganisationType.Partnership, addedOrganisation.OrganisationType);
        }
    }
}
