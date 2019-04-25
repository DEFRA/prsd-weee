namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.ObligatedSentOn
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.DataAccess;
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class EditSentOnAatfSiteHandlerTests
    {
        private readonly WeeeContext context;
        private readonly IReturnDataAccess returnDataAccess;
        private readonly IWeeeAuthorization authorization;
        private readonly ISentOnAatfSiteDataAccess sentOnDataAccess;
        private readonly IAatfSiteDataAccess offSiteDataAccess;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IOrganisationDetailsDataAccess organisationDetailsDataAccess;
        private readonly EditSentOnAatfSiteHandler handler;

        public EditSentOnAatfSiteHandlerTests()
        {
            context = A.Fake<WeeeContext>();
            authorization = A.Fake<IWeeeAuthorization>();
            returnDataAccess = A.Fake<IReturnDataAccess>();
            sentOnDataAccess = A.Fake<ISentOnAatfSiteDataAccess>();
            offSiteDataAccess = A.Fake<IAatfSiteDataAccess>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            organisationDetailsDataAccess = A.Fake<IOrganisationDetailsDataAccess>();

            handler = new EditSentOnAatfSiteHandler(context, authorization, sentOnDataAccess, genericDataAccess, returnDataAccess, organisationDetailsDataAccess, offSiteDataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            var handler = new EditSentOnAatfSiteHandler(context, authorization, sentOnDataAccess, genericDataAccess, returnDataAccess, organisationDetailsDataAccess, offSiteDataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<EditSentOnAatfSite>());

            await action.Should().ThrowAsync<SecurityException>();
        }
        /*
        [Fact]
        public async Task HandleAsync_GivenEditSentOnAatfSiteRequest_DataAccessIsCalled()
        {
            var request = new EditSentOnAatfSite()
            {
                WeeeSentOnId = Guid.NewGuid(),
                OperatorAddressId = Guid.NewGuid(),
                OperatorAddressData = new OperatorAddressData()
                {
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

            var test = new OperatorAddressData()
            {
                Name = request.OperatorAddressData.Name,
                Address1 = request.OperatorAddressData.Address1,
                Address2 = request.OperatorAddressData.Address2,
                TownOrCity = request.OperatorAddressData.TownOrCity,
                CountyOrRegion = request.OperatorAddressData.CountyOrRegion,
                Postcode = request.OperatorAddressData.Postcode,
                CountryName = request.OperatorAddressData.CountryName,
                CountryId = request.OperatorAddressData.CountryId
            };

            var value = A.Fake<AatfAddress>();
            var country = new Country(A.Dummy<Guid>(), A.Dummy<string>());

            A.CallTo(() => genericDataAccess.GetById<AatfAddress>(request.OperatorAddressId)).Returns(value);
            A.CallTo(() => organisationDetailsDataAccess.FetchCountryAsync(request.OperatorAddressData.CountryId)).Returns(country);

            await handler.HandleAsync(request);

            A.CallTo(() => offSiteDataAccess.Update(value, test, country)).MustHaveHappened(Repeated.Exactly.Once);
        }
        */
    }
}
