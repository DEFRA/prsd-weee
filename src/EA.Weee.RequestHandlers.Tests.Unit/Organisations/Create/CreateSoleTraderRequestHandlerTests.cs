namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations.Create
{
    using System.Security;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.Organisation;
    using FakeItEasy;
    using Prsd.Core.Domain;
    using RequestHandlers.Organisations.Create;
    using Requests.Organisations.Create;
    using Xunit;

    public class CreateSoleTraderRequestHandlerTests : CreateOrganisationRequestHandlerTestsBase
    {
        [Fact]
        public async Task CreateSoleTraderRequestHandler_NotExternalUser_ThrowsSecurityException()
        {
            var handler = new CreateSoleTraderRequestHandler(denyingAuthorization, A.Dummy<WeeeContext>(), A.Dummy<IUserContext>());

            await Assert.ThrowsAsync<SecurityException>(async () => await handler.HandleAsync(A.Dummy<CreateSoleTraderRequest>()));
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
