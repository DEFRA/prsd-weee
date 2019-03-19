namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.ObligatedReused
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedReused;
    using EA.Weee.RequestHandlers.AatfReturn.Specification;
    using EA.Weee.RequestHandlers.Organisations;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Common;
    using Xunit;

    public class AddAatfSiteRequestHandlerTests
    {
        private readonly IWeeeAuthorization authorisation;
        private readonly IAddAatfSiteDataAccess addAatfSiteDataAccess;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IObligatedReusedDataAccess obligatedReusedDataAccess; 
        private readonly IOrganisationDetailsDataAccess organisationDetailsDataAccess; 
        private readonly WeeeContext weeeContext;
        private readonly AddAatfSiteHandler handler;

        public AddAatfSiteRequestHandlerTests()
        {
            authorisation = A.Fake<IWeeeAuthorization>();
            addAatfSiteDataAccess = A.Fake<IAddAatfSiteDataAccess>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            obligatedReusedDataAccess = A.Fake<IObligatedReusedDataAccess>();
            organisationDetailsDataAccess = A.Fake<IOrganisationDetailsDataAccess>();
            weeeContext = A.Fake<WeeeContext>();

            handler = new AddAatfSiteHandler(weeeContext, authorisation, addAatfSiteDataAccess, genericDataAccess, organisationDetailsDataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            var handler = new AddAatfSiteHandler(weeeContext, authorization, addAatfSiteDataAccess, genericDataAccess, organisationDetailsDataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<AddAatfSite>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_WithValidInput_SubmitIsCalledCorrectly()
        {
            var siteRequest = AddAatfSiteRequest();
            var weeReused = new WeeeReused(siteRequest.AatfId, siteRequest.ReturnId);
            var country = new Country(A.Dummy<Guid>(), A.Dummy<string>());

            A.CallTo(() => organisationDetailsDataAccess.FetchCountryAsync(siteRequest.AddressData.CountryId)).Returns(country);
            A.CallTo(() => genericDataAccess.GetManyByExpression(
                A<WeeeReusedByAatfIdAndReturnIdSpecification>.That.Matches(w =>
                    w.AatfId == siteRequest.AatfId && w.ReturnId == siteRequest.ReturnId))).Returns(new List<WeeeReused>() { weeReused });

            await handler.HandleAsync(siteRequest);

            A.CallTo(() => addAatfSiteDataAccess.Submit(A<WeeeReusedSite>.That.Matches(w => 
                w.Address.Address1.Equals(siteRequest.AddressData.Address1)
                && w.Address.Address2.Equals(siteRequest.AddressData.Address2)
                && w.Address.CountyOrRegion.Equals(siteRequest.AddressData.CountyOrRegion)
                && w.Address.TownOrCity.Equals(siteRequest.AddressData.TownOrCity)
                && w.Address.Postcode.Equals(siteRequest.AddressData.Postcode)
                && w.Address.Country.Equals(country)                                 
                && w.WeeeReused.Equals(weeReused)))).MustHaveHappened(Repeated.Exactly.Once);
        }

        private AddAatfSite AddAatfSiteRequest()
        {
            var siteRequest = new AddAatfSite()
            {
                AatfId = Guid.NewGuid(),
                ReturnId = Guid.NewGuid(),
                AddressData = new SiteAddressData()
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
