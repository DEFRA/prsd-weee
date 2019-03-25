namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.ObligatedSentOn
{
    using EA.Prsd.Core.Mapper;
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
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;

    public class EditSentOnAatfSiteHandlerTests
    {
        private readonly WeeeContext context;
        private readonly IWeeeAuthorization authorization;
        private readonly ISentOnAatfSiteDataAccess sentOnDataAccess;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IOrganisationDetailsDataAccess orgDataAccess;
        private readonly IReturnDataAccess returnDataAccess;
        private readonly EditSentOnAatfSiteHandler handler;
        private readonly IMap<AatfAddress, AatfAddressData> mapper;

        public EditSentOnAatfSiteHandlerTests()
        {
            context = A.Fake<WeeeContext>();
            authorization = A.Fake<IWeeeAuthorization>();
            orgDataAccess = A.Fake<IOrganisationDetailsDataAccess>();
            sentOnDataAccess = A.Fake<ISentOnAatfSiteDataAccess>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            returnDataAccess = A.Fake<IReturnDataAccess>();
            mapper = A.Fake<IMap<AatfAddress, AatfAddressData>>();

            handler = new EditSentOnAatfSiteHandler(context, authorization, sentOnDataAccess, genericDataAccess, returnDataAccess, orgDataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            var handler = new GetSentOnAatfSiteHandler(authorization, sentOnDataAccess, mapper);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<GetSentOnAatfSite>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenEditSentOnAatfSiteRequest_DataAccessIsCalled()
        {
            var operatorAddressData = new OperatorAddressData()
            {
                Name = "OpName",
                Address1 = "Address1",
                Address2 = "Address2",
                TownOrCity = "Town",
                CountyOrRegion = "County",
                Postcode = "GU22 7UY",
                CountryId = Guid.NewGuid()
            };

            var request = new EditSentOnAatfSite()
            {
                WeeeSentOnId = Guid.NewGuid(),
                OperatorAddressData = operatorAddressData
            };

            var weeeSentOn = new WeeeSentOn();
            var country = new Country(A.Dummy<Guid>(), A.Dummy<string>());

            A.CallTo(() => genericDataAccess.GetById<WeeeSentOn>(request.WeeeSentOnId)).Returns(weeeSentOn);
            A.CallTo(() => orgDataAccess.FetchCountryAsync(request.OperatorAddressData.CountryId)).Returns(country);

            var operatorAddress = new AatfAddress(
            request.OperatorAddressData.Name,
            request.OperatorAddressData.Address1,
            request.OperatorAddressData.Address2,
            request.OperatorAddressData.TownOrCity,
            request.OperatorAddressData.CountyOrRegion,
            request.OperatorAddressData.Postcode,
            country);

            await handler.HandleAsync(request);

            A.CallTo(() => sentOnDataAccess.UpdateWithOperatorAddress(weeeSentOn, operatorAddress)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
