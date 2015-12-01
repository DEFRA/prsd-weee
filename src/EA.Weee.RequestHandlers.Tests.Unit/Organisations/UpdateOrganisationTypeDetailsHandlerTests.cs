namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.Organisation;
    using FakeItEasy;
    using RequestHandlers.Organisations;
    using RequestHandlers.Security;
    using Requests.Organisations;
    using Weee.Tests.Core;
    using Xunit;
    using OrganisationType = Core.Organisations.OrganisationType;

    public class UpdateOrganisationTypeDetailsHandlerTests
    {
        private readonly DbContextHelper dbHelper = new DbContextHelper();

        private readonly OrganisationHelper orgHelper = new OrganisationHelper();

        [Fact]
        public async Task UpdateOrganisationTypeDetailsHandler_NotOrganisationUser_ThrowsSecurityException()
        {
            var authorization = AuthorizationBuilder.CreateUserDeniedFromAccessingOrganisation();

            var handler = new UpdateOrganisationTypeDetailsHandler(A<WeeeContext>._, authorization);
            var message = new UpdateOrganisationTypeDetails(Guid.NewGuid(), OrganisationType.RegisteredCompany, A<string>._, A<string>._, A<string>._);

            await Assert.ThrowsAsync<SecurityException>(async () => await handler.HandleAsync(message));
        }

        [Fact]
        public async Task UpdateOrganisationTypeDetailsHandler_NoSuchOrganisation_ThrowsArgumentException()
        {
            WeeeContext context = A.Fake<WeeeContext>();
            A.CallTo(() => context.Organisations).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Organisation>()));

            var authorization = AuthorizationBuilder.CreateUserAllowedToAccessOrganisation();

            var handler = new UpdateOrganisationTypeDetailsHandler(context, authorization);
            var message = new UpdateOrganisationTypeDetails(Guid.NewGuid(), OrganisationType.RegisteredCompany, A<string>._, A<string>._, A<string>._);

            var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await handler.HandleAsync(message));

            Assert.True(exception.Message.ToUpperInvariant().Contains("COULD NOT FIND"));
            Assert.True(exception.Message.ToUpperInvariant().Contains("ORGANISATION"));
            Assert.True(exception.Message.Contains(message.OrganisationId.ToString()));
        }

        [Fact]
        public async Task UpdateOrganisationTypeDetailsHandler_UpdateOrganisationTypeDetails_ReturnsUpdatedOrganisation()
        {
            var organisations = MakeOrganisation();

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.Organisations).Returns(organisations);

            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserAllowedToAccessOrganisation();

            var handler = new UpdateOrganisationTypeDetailsHandler(context, authorization);

            const string tradingName = "trading name";
            const string companyName = "company name";
            const string companyRegistrationNumber = "12345678";

            await
                handler.HandleAsync(new UpdateOrganisationTypeDetails(organisations.FirstOrDefault().Id,
                    OrganisationType.SoleTraderOrIndividual, companyName, tradingName, companyRegistrationNumber));

            var orgInfo = organisations.FirstOrDefault();

            Assert.NotNull(orgInfo);
            Assert.Equal(orgInfo.OrganisationType, Domain.Organisation.OrganisationType.SoleTraderOrIndividual);
            Assert.Equal(orgInfo.Name, companyName);
            Assert.Equal(orgInfo.TradingName, tradingName);
            Assert.Equal(orgInfo.CompanyRegistrationNumber, companyRegistrationNumber);
        }

        private DbSet<Organisation> MakeOrganisation()
        {
            return dbHelper.GetAsyncEnabledDbSet(new[]
            {
                orgHelper.GetOrganisationWithName("TEST Ltd")
            });
        }
    }
}
