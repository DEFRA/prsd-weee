namespace EA.Weee.RequestHandlers.Tests.DataAccess.Scheme
{
    using Domain;
    using Domain.Organisation;
    using Domain.Scheme;
    using EA.Weee.DataAccess;
    using FakeItEasy;
    using Prsd.Core.Domain;
    using RequestHandlers.Scheme;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class GetSchemesDataAccessTests
    {
        [Fact]
        public async void GetSchemesDataAccess_GetCompleteSchemes_ReturnsOnlySchemesWithCompleteOrganisations()
        {
            var userContext = A.Fake<IUserContext>();
            A.CallTo(() => userContext.UserId).Returns(Guid.NewGuid());

            IEventDispatcher eventDispatcher = A.Fake<IEventDispatcher>();

            var realWeeeContext = new WeeeContext(userContext, eventDispatcher);

            var testSchemes = await AddTestOrganisationsAndSchemes(realWeeeContext);
            var completeOrganisation = testSchemes[OrganisationStatus.Complete].Organisation;
            var incompleteOrganisation = testSchemes[OrganisationStatus.Incomplete].Organisation;
            
            var dataAccess = new GetSchemesDataAccess(realWeeeContext);
            var result = await dataAccess.GetCompleteSchemes();

            Assert.All(result, s => Assert.Equal(s.Organisation.OrganisationStatus.Value, OrganisationStatus.Complete.Value));
            Assert.True(result.Any(s => s.Organisation.TradingName == completeOrganisation.TradingName));
            Assert.False(result.Any(s => s.Organisation.TradingName == incompleteOrganisation.TradingName));

            Cleanup(realWeeeContext, testSchemes.Values.ToList());
        }

        private void Cleanup(WeeeContext context, List<Scheme> schemesToRemove)
        {
            context.Organisations.RemoveRange(schemesToRemove.Select(s => s.Organisation));
            context.Schemes.RemoveRange(schemesToRemove);
        }

        private async Task<Dictionary<OrganisationStatus, Scheme>> AddTestOrganisationsAndSchemes(WeeeContext context)
        {
            var completeOrganisation = CreateTestOrganisation(context, "GetSchemesDataAccessTests - Complete organisation");
            completeOrganisation.CompleteRegistration();
            var incompleteOrganisation = CreateTestOrganisation(context, "GetSchemesDataAccessTests - Incomplete organisation");

            context.Organisations.Add(completeOrganisation);
            context.Organisations.Add(incompleteOrganisation);
            await context.SaveChangesAsync();

            var completeScheme = new Scheme(completeOrganisation.Id);
            var incompleteScheme = new Scheme(incompleteOrganisation.Id);

            context.Schemes.Add(completeScheme);
            context.Schemes.Add(incompleteScheme);
            await context.SaveChangesAsync();

            return new Dictionary<OrganisationStatus, Scheme>
            {
                {OrganisationStatus.Complete, completeScheme},
                {OrganisationStatus.Incomplete, incompleteScheme},
            };
        }

        private Organisation CreateTestOrganisation(WeeeContext context, string tradingName)
        {
            var organisation = Organisation.CreateSoleTrader(tradingName);

            organisation.AddOrUpdateAddress(
                AddressType.OrganisationAddress,
                new Address(
                    "Address 1",
                    "Address 2",
                    "Town",
                    "County",
                    "TEST123",
                    context.Countries.First(),
                    "01234 567890",
                    "test@test.test"));

            organisation.AddOrUpdateMainContactPerson(new Contact("Test first name", "Test last name", "Test position"));

            return organisation;
        }
    }
}