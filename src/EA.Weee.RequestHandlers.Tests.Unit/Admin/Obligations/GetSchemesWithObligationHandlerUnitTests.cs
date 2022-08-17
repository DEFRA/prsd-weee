namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Obligations
{
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using Core.Scheme;
    using DataAccess.DataAccess;
    using Domain.Scheme;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using RequestHandlers.Admin.Obligations;
    using RequestHandlers.Security;
    using Requests.Admin.Obligations;
    using System.Security;
    using System.Threading.Tasks;
    using Weee.Tests.Core;
    using Xunit;

    public class GetSchemesWithObligationHandlerUnitTests : SimpleUnitTestBase
    {
        private GetSchemesWithObligationHandler handler;
        private readonly GetSchemesWithObligation request;
        private readonly IWeeeAuthorization authorization;
        private readonly IObligationDataAccess obligationDataAccess;
        private readonly IMap<Scheme, SchemeData> schemeMap;

        public GetSchemesWithObligationHandlerUnitTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            obligationDataAccess = A.Fake<IObligationDataAccess>();
            schemeMap = A.Fake<IMap<Scheme, SchemeData>>();

            request = new GetSchemesWithObligation(TestFixture.Create<int>());

            handler = new GetSchemesWithObligationHandler(authorization, obligationDataAccess, schemeMap);
        }

        [Fact]
        public async Task HandleAsync_InternalAccess_ShouldBeChecked()
        {
            //act
            await handler.HandleAsync(request);

            //arrange
            A.CallTo(() => authorization.EnsureCanAccessInternalArea()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_NoInternalAccess_ThrowsSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            handler = new GetSchemesWithObligationHandler(authorization, obligationDataAccess, schemeMap);

            //act
            var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(request));

            //assert
            exception.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ObligationComplianceYearsShouldBeRetrieved()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => obligationDataAccess.GetObligationSchemeData(null, request.ComplianceYear)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestAndSchemesWithObligations_OnlyDistinctSchemesShouldBeMapped()
        {
            //arrange
            var scheme1 = A.Fake<Scheme>();
            var scheme2 = A.Fake<Scheme>();
            var schemes = new List<Scheme>()
            {
                scheme1,
                scheme2,
                scheme2
            };

            A.CallTo(() => obligationDataAccess.GetObligationSchemeData(null, A<int>._)).Returns(schemes);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => schemeMap.Map(scheme1)).MustHaveHappenedOnceExactly();
            A.CallTo(() => schemeMap.Map(scheme2)).MustHaveHappenedOnceExactly();
            A.CallTo(() => schemeMap.Map(A<Scheme>._)).MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestAndSchemesWithObligations_SchemesShouldBeMapped()
        {
            //arrange
            var schemes = new List<Scheme>()
            {
                A.Fake<Scheme>(),
                A.Fake<Scheme>()
            };

            A.CallTo(() => obligationDataAccess.GetObligationSchemeData(null, A<int>._)).Returns(schemes);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => schemeMap.Map(schemes.ElementAt(0))).MustHaveHappenedOnceExactly();
            A.CallTo(() => schemeMap.Map(schemes.ElementAt(1))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestAndMappedSchemes_MappedSchemesShouldBeReturned()
        {
            //arrange
            var schemes = new List<Scheme>()
            {
                A.Fake<Scheme>(),
                A.Fake<Scheme>()
            };

            var schemeData = new List<SchemeData>()
            {
                A.Fake<SchemeData>(),
                A.Fake<SchemeData>()
            };

            A.CallTo(() => obligationDataAccess.GetObligationSchemeData(null, A<int>._)).Returns(schemes);
            A.CallTo(() => schemeMap.Map(A<Scheme>._)).ReturnsNextFromSequence(schemeData.ToArray());

            //act
            var result = await handler.HandleAsync(request);

            //assert
            result.Should().Contain(schemeData.ElementAt(0));
            result.Should().Contain(schemeData.ElementAt(1));
            result.Should().HaveCount(2);
        }
    }
}
