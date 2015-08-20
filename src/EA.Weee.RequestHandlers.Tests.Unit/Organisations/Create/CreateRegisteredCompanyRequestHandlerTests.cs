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

    public class CreateRegisteredCompanyRequestHandlerTests : CreateOrganisationRequestHandlerTestsBase
    {
        [Fact]
        public async Task NotExternalUser_ThrowsSecurityException()
        {
            var auth = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            var handler = new CreateRegisteredCompanyRequestHandler(auth, A<WeeeContext>._, A<IUserContext>._);

            await Assert.ThrowsAsync<SecurityException>(async () => await handler.HandleAsync(A<CreateRegisteredCompanyRequest>._));
        }

        [Fact]
        public async Task HappyPath_CreatesPartnershipAndApprovedOrganisationUser()
        {
            var auth = new AuthorizationBuilder().AllowExternalAreaAccess().Build();

            var context = GetPreparedContext();

            var userId = Guid.NewGuid();
            var userContext = A.Fake<IUserContext>();
            A.CallTo(() => userContext.UserId).Returns(userId);

            const string BusinessName = "Some business name";
            const string CompanyRegistrationNumber = "Some CRN";
            const string TradingName = "Some trading name";

            var handler = new CreateRegisteredCompanyRequestHandler(auth, context, userContext);

            await handler.HandleAsync(new CreateRegisteredCompanyRequest
            {
                BusinessName = BusinessName,
                CompanyRegistrationNumber = CompanyRegistrationNumber,
                TradingName = TradingName
            });

            Assert.NotNull(addedOrganisation);
            Assert.NotNull(addedOrganisationUser);
            Assert.NotEqual(Guid.Empty, addedOrganisationId);

            Assert.Equal(BusinessName, addedOrganisation.Name);
            Assert.Equal(CompanyRegistrationNumber, addedOrganisation.CompanyRegistrationNumber);
            Assert.Equal(TradingName, addedOrganisation.TradingName);
            Assert.Equal(OrganisationType.RegisteredCompany, addedOrganisation.OrganisationType);
            Assert.Equal(addedOrganisationId, addedOrganisationUser.OrganisationId);
            Assert.Equal(UserStatus.Approved, addedOrganisationUser.UserStatus);
        }
    }
}
