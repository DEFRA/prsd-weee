namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.ObligatedReceived
{
    using Domain.AatfReturn;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.RequestHandlers.Organisations;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.AatfReturn;
    using RequestHandlers.AatfReturn.ObligatedReused;
    using RequestHandlers.Security;
    using Requests.AatfReturn.Obligated;
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using Weee.Tests.Core;
    using Xunit;

    public class EditAatfSiteHandlerTests
    {
        private readonly IWeeeAuthorization authorisation;
        private readonly IAatfSiteDataAccess aatfSiteDataAccess;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IOrganisationDetailsDataAccess organisationDetailsDataAccess;
        private readonly EditAatfSiteHandler handler;
        private readonly WeeeContext weeeContext;

        public EditAatfSiteHandlerTests()
        {
            authorisation = A.Fake<IWeeeAuthorization>();
            aatfSiteDataAccess = A.Fake<IAatfSiteDataAccess>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            organisationDetailsDataAccess = A.Fake<IOrganisationDetailsDataAccess>();
            weeeContext = A.Fake<WeeeContext>();

            handler = new EditAatfSiteHandler(weeeContext, authorisation, aatfSiteDataAccess, genericDataAccess, organisationDetailsDataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            var handler = new EditAatfSiteHandler(weeeContext, authorization, aatfSiteDataAccess, genericDataAccess, organisationDetailsDataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<EditAatfSite>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenMessageContainingUpdatedAddress_DetailsAreUpdatedCorrectly()
        {
            var updateRequest = new EditAatfSite()
            {
                AddressData = new SiteAddressData()
                {
                    Name = "Name",
                    Address1 = "Address1",
                    Address2 = "Address2",
                    TownOrCity = "Town",
                    CountyOrRegion = "County",
                    Postcode = "Postcode",
                    CountryId = Guid.NewGuid(),
                    Id = Guid.NewGuid()
                }
            };

            var returnAdress = A.Fake<AatfAddress>();

            var country = new Country(A.Dummy<Guid>(), A.Dummy<string>());

            A.CallTo(() => organisationDetailsDataAccess.FetchCountryAsync(updateRequest.AddressData.CountryId)).Returns(country);
            A.CallTo(() => genericDataAccess.GetById<AatfAddress>(updateRequest.AddressData.Id)).Returns(returnAdress);

            await handler.HandleAsync(updateRequest);

            A.CallTo(() => aatfSiteDataAccess.Update(returnAdress, updateRequest.AddressData, country)).MustHaveHappened(1, Times.Exactly);
        }
    }
}
