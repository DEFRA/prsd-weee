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

    public class CreateRegisteredCompanyRequestHandlerTests : CreateOrganisationRequestHandlerTestsBase
    {
        [Fact]
        public async Task CreateRegisteredCompanyRequestHandler_NotExternalUser_ThrowsSecurityException()
        {
            var handler = new CreateRegisteredCompanyRequestHandler(denyingAuthorization, A<WeeeContext>._, A<IUserContext>._);

            await Assert.ThrowsAsync<SecurityException>(async () => await handler.HandleAsync(A<CreateRegisteredCompanyRequest>._));
        }

        [Fact]
        public async Task CreateRegisteredCompanyRequestHandler_HappyPath_CreatesPartnershipAndApprovedOrganisationUser()
        {
            var context = GetPreparedContext();
            var userContext = GetPreparedUserContext();

            const string BusinessName = "Some business name";
            const string CompanyRegistrationNumber = "Some CRN";
            const string TradingName = "Some trading name";

            var handler = new CreateRegisteredCompanyRequestHandler(permissiveAuthorization, context, userContext);
            await handler.HandleAsync(new CreateRegisteredCompanyRequest
            {
                BusinessName = BusinessName,
                CompanyRegistrationNumber = CompanyRegistrationNumber,
                TradingName = TradingName
            });

            DoSharedAssertions(TradingName);
            Assert.Equal(BusinessName, addedOrganisation.Name);
            Assert.Equal(CompanyRegistrationNumber, addedOrganisation.CompanyRegistrationNumber);
            Assert.Equal(OrganisationType.RegisteredCompany, addedOrganisation.OrganisationType);
        }
    }
}
