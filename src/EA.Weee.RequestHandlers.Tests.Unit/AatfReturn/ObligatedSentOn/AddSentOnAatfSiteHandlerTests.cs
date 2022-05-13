namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.ObligatedSentOn
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn;
    using EA.Weee.RequestHandlers.Organisations;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using Xunit;

    public class AddSentOnAatfSiteHandlerTests
    {
        private readonly IReturnDataAccess returnDataAccess;
        private readonly IWeeeSentOnDataAccess sentOnDataAccess;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IOrganisationDetailsDataAccess organisationDetailsDataAccess;
        private readonly AddSentOnAatfSiteHandler handler;

        public AddSentOnAatfSiteHandlerTests()
        {
            returnDataAccess = A.Fake<IReturnDataAccess>();
            var authorization = A.Fake<IWeeeAuthorization>();
            sentOnDataAccess = A.Fake<IWeeeSentOnDataAccess>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            organisationDetailsDataAccess = A.Fake<IOrganisationDetailsDataAccess>();

            handler = new AddSentOnAatfSiteHandler(authorization, sentOnDataAccess, genericDataAccess, returnDataAccess, organisationDetailsDataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            var handler = new AddSentOnAatfSiteHandler(authorization, sentOnDataAccess, genericDataAccess, returnDataAccess, organisationDetailsDataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<AddSentOnAatfSite>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_WithValidInput_SubmitIsCalledCorrectly()
        {
            var siteRequest = AddSentOnAatfSiteRequest();
            var country = new Country(A.Dummy<Guid>(), A.Dummy<string>());
            var countryOP = new Country(A.Dummy<Guid>(), A.Dummy<string>());

            A.CallTo(() => organisationDetailsDataAccess.FetchCountryAsync(siteRequest.SiteAddressData.CountryId)).Returns(country);
            A.CallTo(() => organisationDetailsDataAccess.FetchCountryAsync(siteRequest.OperatorAddressData.CountryId)).Returns(countryOP);

            await handler.HandleAsync(siteRequest);

            A.CallTo(() => sentOnDataAccess.Submit(A<WeeeSentOn>.That.Matches(w => w.SiteAddress.Address1.Equals(siteRequest.SiteAddressData.Address1)
            && w.SiteAddress.Address2.Equals(siteRequest.SiteAddressData.Address2)
            && w.SiteAddress.CountyOrRegion.Equals(siteRequest.SiteAddressData.CountyOrRegion)
            && w.SiteAddress.Postcode.Equals(siteRequest.SiteAddressData.Postcode)
            && w.SiteAddress.TownOrCity.Equals(siteRequest.SiteAddressData.TownOrCity)
            && w.SiteAddress.Country.Equals(country)
            && w.OperatorAddress.Name.Equals(siteRequest.OperatorAddressData.Name)
            && w.OperatorAddress.Address2.Equals(siteRequest.OperatorAddressData.Address2)
            && w.OperatorAddress.CountyOrRegion.Equals(siteRequest.OperatorAddressData.CountyOrRegion)
            && w.OperatorAddress.Postcode.Equals(siteRequest.OperatorAddressData.Postcode)
            && w.OperatorAddress.TownOrCity.Equals(siteRequest.OperatorAddressData.TownOrCity)
            && w.OperatorAddress.Country.Equals(countryOP)))).MustHaveHappened(1, Times.Exactly);
        }

        private AddSentOnAatfSite AddSentOnAatfSiteRequest()
        {
            var siteRequest = new AddSentOnAatfSite()
            {
                AatfId = Guid.NewGuid(),
                ReturnId = Guid.NewGuid(),
                OrganisationId = Guid.NewGuid(),
                SiteAddressData = new AatfAddressData()
                {
                    CountryId = Guid.NewGuid(),
                    Address1 = "address1",
                    Address2 = "address2",
                    CountyOrRegion = "county",
                    Name = "name",
                    Postcode = "postcode",
                    TownOrCity = "town"
                },
                OperatorAddressData = new OperatorAddressData()
                {
                    CountryId = Guid.NewGuid(),
                    Address1 = "address1OP",
                    Address2 = "address2OP",
                    CountyOrRegion = "countyOP",
                    Name = "nameOP",
                    Postcode = "postcodeOP",
                    TownOrCity = "townOP"
                }
            };
            return siteRequest;
        }
    }
}
