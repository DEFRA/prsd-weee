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

    public class CreateSoleTraderRequestHandlerTests : CreateOrganisationRequestHandlerTestsBase
    {
        [Fact]
        public async Task CreateSoleTraderRequestHandler_NotExternalUser_ThrowsSecurityException()
        {
            var handler = new CreateSoleTraderRequestHandler(denyingAuthorization, A<WeeeContext>._, A<IUserContext>._);

            await Assert.ThrowsAsync<SecurityException>(async () => await handler.HandleAsync(A<CreateSoleTraderRequest>._));
        }

        [Fact]
        public async Task CreateSoleTraderRequestHandler_HappyPath_CreatesPartnershipAndApprovedOrganisationUser()
        {
            var context = GetPreparedContext();
            var userContext = GetPreparedUserContext();

            const string TradingName = "Some trading name";

            var handler = new CreateSoleTraderRequestHandler(permissiveAuthorization, context, userContext);
            await handler.HandleAsync(new CreateSoleTraderRequest { TradingName = TradingName });

            DoSharedAssertions(TradingName);
            Assert.Equal(OrganisationType.SoleTraderOrIndividual, addedOrganisation.OrganisationType);
        }
    }
}
