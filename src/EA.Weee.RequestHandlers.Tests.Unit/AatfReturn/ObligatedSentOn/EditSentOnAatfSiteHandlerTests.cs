namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.ObligatedSentOn
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedReused;
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

    public class EditSentOnAatfSiteHandlerTests
    {
        private readonly IAatfSiteDataAccess offSiteDataAccess;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IOrganisationDetailsDataAccess organisationDetailsDataAccess;
        private readonly EditSentOnAatfSiteHandler handler;

        public EditSentOnAatfSiteHandlerTests()
        {
            var authorization = A.Fake<IWeeeAuthorization>();
            offSiteDataAccess = A.Fake<IAatfSiteDataAccess>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            organisationDetailsDataAccess = A.Fake<IOrganisationDetailsDataAccess>();

            handler = new EditSentOnAatfSiteHandler(authorization, genericDataAccess, organisationDetailsDataAccess, offSiteDataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            var handler = new EditSentOnAatfSiteHandler(authorization, genericDataAccess, organisationDetailsDataAccess, offSiteDataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<EditSentOnAatfSite>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenSiteAddressDataInRequest_AddressDataShouldBeOfTypeSiteAddressData()
        {
            var request = CreateRequest();

            var value = A.Fake<AatfAddress>();
            var country = new Country(A.Dummy<Guid>(), A.Dummy<string>());

            A.CallTo(() => genericDataAccess.GetById<AatfAddress>(request.SiteAddressData.Id)).Returns(value);
            A.CallTo(() => organisationDetailsDataAccess.FetchCountryAsync(request.SiteAddressData.CountryId)).Returns(country);

            await handler.HandleAsync(request);

            A.CallTo(() => offSiteDataAccess.Update(value, A<SiteAddressData>._, country)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task HandleAsync_GivenEditSentOnAatfSiteRequest_DataAccessIsCalled()
        {
            var request = CreateRequest();

            var value = A.Fake<AatfAddress>();
            var country = new Country(A.Dummy<Guid>(), A.Dummy<string>());

            A.CallTo(() => genericDataAccess.GetById<AatfAddress>(request.OperatorAddressData.Id)).Returns(value);
            A.CallTo(() => organisationDetailsDataAccess.FetchCountryAsync(request.OperatorAddressData.CountryId)).Returns(country);

            await handler.HandleAsync(request);

            A.CallTo(() => offSiteDataAccess.Update(value, A<OperatorAddressData>.That.Matches(o => o.Name == request.OperatorAddressData.Name
                && o.Address1 == request.OperatorAddressData.Address1
                && o.Address2 == request.OperatorAddressData.Address2
                && o.TownOrCity == request.OperatorAddressData.TownOrCity
                && o.CountyOrRegion == request.OperatorAddressData.CountyOrRegion
                && o.Postcode == request.OperatorAddressData.Postcode
                && o.CountryName == request.OperatorAddressData.CountryName
                && o.CountryId == request.OperatorAddressData.CountryId), country)).MustHaveHappened(1, Times.Exactly);
        }

        private static EditSentOnAatfSite CreateRequest()
        {
            var request = new EditSentOnAatfSite()
            {
                WeeeSentOnId = Guid.NewGuid(),
                SiteAddressData = new AatfAddressData()
                {
                    Id = Guid.NewGuid(),
                    Name = "OpName",
                    Address1 = "Address1",
                    Address2 = "Address2",
                    TownOrCity = "Town",
                    CountyOrRegion = "County",
                    Postcode = "GU22 7UY",
                    CountryName = "France",
                    CountryId = Guid.NewGuid()
                },
                OperatorAddressData = new OperatorAddressData()
                {
                    Id = Guid.NewGuid(),
                    Name = "OpName",
                    Address1 = "Address1",
                    Address2 = "Address2",
                    TownOrCity = "Town",
                    CountyOrRegion = "County",
                    Postcode = "GU22 7UY",
                    CountryName = "France",
                    CountryId = Guid.NewGuid()
                }
            };
            return request;
        }
    }
}
