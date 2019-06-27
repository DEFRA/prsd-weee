namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Domain.Scheme;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Scheme;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.Scheme;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Scheme;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;
    using ObligationType = Domain.Obligation.ObligationType;
    using SchemeStatus = Domain.Scheme.SchemeStatus;

    public class GetSchemesExternalHandlerTests
    {
        private readonly RequestHandlers.Scheme.IGetSchemesDataAccess dataAccess;
        private readonly IMap<Scheme, SchemeData> schemeMap;
        private readonly IWeeeAuthorization authorization;
        private GetSchemesExternalHandler handler;
        private readonly WeeeContext context;
        private SchemeData schemeData1;
        private SchemeData schemeData2;
        private SchemeData schemeData3;
        public GetSchemesExternalHandlerTests()
        {
            dataAccess = A.Fake<RequestHandlers.Scheme.IGetSchemesDataAccess>();
            schemeMap = A.Fake<IMap<Scheme, SchemeData>>();
            authorization = A.Fake<IWeeeAuthorization>();
            context = A.Fake<WeeeContext>();

            handler = new GetSchemesExternalHandler(dataAccess, schemeMap, authorization);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorizationDeny = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            handler = new GetSchemesExternalHandler(A.Fake<RequestHandlers.Scheme.IGetSchemesDataAccess>(), A.Dummy<IMap<Scheme, SchemeData>>(), authorizationDeny);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<GetSchemesExternal>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenGetSchemeExternalRequest_ListOfSchemesShouldBeReturned()
        {
            var request = new GetSchemesExternal();

            var result = await handler.HandleAsync(request);

            A.CallTo(() => dataAccess.GetCompleteSchemes()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenGetSchemeExternalRequest_ReturnsSchemesSortedBySchemeNameAsync()
        {
            var request = new GetSchemesExternal();

            Organisation organisation = Organisation.CreateSoleTrader("Test Organisation");
            Scheme scheme1 = new Scheme(organisation);
            scheme1.UpdateScheme("Scheme D", "WEE/11AAAA11/SCH", "WEE1234567", ObligationType.Both, A.Dummy<UKCompetentAuthority>());
            scheme1.SetStatus(SchemeStatus.Approved);

            Scheme scheme2 = new Scheme(organisation);
            scheme2.UpdateScheme("Scheme A", "WEE/11AAAA11/SCH", "WEE1234567", ObligationType.Both, A.Dummy<UKCompetentAuthority>());
            scheme2.SetStatus(SchemeStatus.Approved);

            Scheme scheme3 = new Scheme(organisation);
            scheme3.UpdateScheme("Scheme C", "WEE/11AAAA11/SCH", "WEE1234567", ObligationType.Both, A.Dummy<UKCompetentAuthority>());
            scheme3.SetStatus(SchemeStatus.Approved);

            var results = new List<Domain.Scheme.Scheme>()
            {
                scheme1,
                scheme2,
                scheme3
            };
            A.CallTo(() => dataAccess.GetCompleteSchemes()).Returns(results);

            IMap<Scheme, SchemeData> schemeMap = A.Fake<IMap<EA.Weee.Domain.Scheme.Scheme, SchemeData>>();

            schemeData1 = A.Fake<SchemeData>();
            schemeData1.SchemeName = "Scheme D";
            schemeData1.SchemeStatus = Core.Shared.SchemeStatus.Approved;
            A.CallTo(() => schemeMap.Map(scheme1)).Returns(schemeData1);

            schemeData2 = A.Fake<SchemeData>();
            schemeData2.SchemeName = "Scheme A";
            schemeData2.SchemeStatus = Core.Shared.SchemeStatus.Approved;
            A.CallTo(() => schemeMap.Map(scheme2)).Returns(schemeData2);

            schemeData3 = A.Fake<SchemeData>();
            schemeData3.SchemeName = "Scheme C";
            schemeData3.SchemeStatus = Core.Shared.SchemeStatus.Approved;
            A.CallTo(() => schemeMap.Map(scheme3)).Returns(schemeData3);

            handler = new GetSchemesExternalHandler(dataAccess, schemeMap, authorization);

            var result = await handler.HandleAsync(request);

            Assert.Collection(
              result,
              (element1) => Assert.Equal("Scheme A", element1.SchemeName),
              (element2) => Assert.Equal("Scheme C", element2.SchemeName),
              (element3) => Assert.Equal("Scheme D", element3.SchemeName));
        }
    }
}
