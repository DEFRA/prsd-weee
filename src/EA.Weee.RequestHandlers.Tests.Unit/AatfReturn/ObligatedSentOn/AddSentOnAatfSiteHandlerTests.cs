namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.ObligatedSentOn
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.DataAccess;
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class AddSentOnAatfSiteHandlerTests
    {
        private readonly WeeeContext context;
        private readonly IReturnDataAccess returnDataAccess;
        private readonly IWeeeAuthorization authorization;
        private readonly IAddSentOnAatfSiteDataAccess sentOnDataAccess;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IOrganisationDetailsDataAccess organisationDetailsDataAccess;
        private readonly AddSentOnAatfSiteHandler handler;

        public AddSentOnAatfSiteHandlerTests()
        {
            context = A.Fake<WeeeContext>();
            returnDataAccess = A.Fake<IReturnDataAccess>();
            authorization = A.Fake<IWeeeAuthorization>();
            sentOnDataAccess = A.Fake<IAddSentOnAatfSiteDataAccess>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            organisationDetailsDataAccess = A.Fake<IOrganisationDetailsDataAccess>();

            handler = new AddSentOnAatfSiteHandler(context, authorization, sentOnDataAccess, genericDataAccess, returnDataAccess, organisationDetailsDataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            var handler = new AddSentOnAatfSiteHandler(context, authorization, sentOnDataAccess, genericDataAccess, returnDataAccess, organisationDetailsDataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<AddSentOnAatfSite>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_WithValidInput_SubmitIsCalledCorrectly()
        {
            var siteRequest = AddSentOnAatfSiteRequest();
            var country = new Country(A.Dummy<Guid>(), A.Dummy<string>());

            A.CallTo(() => organisationDetailsDataAccess.FetchCountryAsync(siteRequest.SiteAddressData.CountryId)).Returns(country);

            await handler.HandleAsync(siteRequest);

            A.CallTo(() => sentOnDataAccess.Submit(A<WeeeSentOn>.That.Matches(w => w.SiteAddress.Address1.Equals(siteRequest.SiteAddressData.Address1)
            && w.SiteAddress.Address2.Equals(siteRequest.SiteAddressData.Address2)
            && w.SiteAddress.CountyOrRegion.Equals(siteRequest.SiteAddressData.CountyOrRegion)
            && w.SiteAddress.Postcode.Equals(siteRequest.SiteAddressData.Postcode)
            && w.SiteAddress.TownOrCity.Equals(siteRequest.SiteAddressData.TownOrCity)
            && w.SiteAddress.Country.Equals(country)))).MustHaveHappened(Repeated.Exactly.Once);
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
                }
            };
            return siteRequest;
        }
    }
}
