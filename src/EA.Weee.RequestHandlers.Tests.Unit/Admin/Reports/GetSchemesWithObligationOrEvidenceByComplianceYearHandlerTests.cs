namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Reports
{
    using System.Linq;
    using AutoFixture;
    using Core.Scheme;
    using DataAccess.DataAccess;
    using Domain.Scheme;
    using FakeItEasy;
    using Prsd.Core.Mapper;
    using RequestHandlers.Admin.GetSchemes;
    using RequestHandlers.Security;
    using Requests.Admin.Reports;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Weee.Tests.Core;
    using Xunit;

    public class GetSchemesWithObligationOrEvidenceByComplianceYearHandlerTests : SimpleUnitTestBase
    {
        private readonly GetSchemesWithObligationOrEvidenceByComplianceYearHandler handler;
        private readonly IWeeeAuthorization authorization;
        private readonly IMap<Scheme, SchemeData> schemeMap;
        private readonly IObligationDataAccess obligationDataAccess;

        public GetSchemesWithObligationOrEvidenceByComplianceYearHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            schemeMap = A.Fake<IMap<Scheme, SchemeData>>();
            obligationDataAccess = A.Fake<IObligationDataAccess>();

            handler = new GetSchemesWithObligationOrEvidenceByComplianceYearHandler(authorization,
                schemeMap,
                obligationDataAccess);
        }

        [Fact]
        public async Task HandleAsync_ShouldCheckInternalAccess()
        {
            //act
            await handler.HandleAsync(TestFixture.Create<GetSchemesWithObligationOrEvidence>());

            //assert
            A.CallTo(() => authorization.EnsureCanAccessInternalArea()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestObligationDataAccessShouldBeCalled()
        {
            //arrange
            var request = TestFixture.Create<GetSchemesWithObligationOrEvidence>();

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => obligationDataAccess.GetSchemesWithObligationOrEvidence(request.ComplianceYear)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenObligationSchemes_SchemesShouldBeMapped()
        {
            //arrange
            var schemes = TestFixture.CreateMany<Scheme>().ToList();
            var request = TestFixture.Create<GetSchemesWithObligationOrEvidence>();

            A.CallTo(() => obligationDataAccess.GetSchemesWithObligationOrEvidence(A<int>._)).Returns(schemes);

            //act
            await handler.HandleAsync(request);

            //assert
            schemes.ForEach(s => A.CallTo(() => schemeMap.Map(s)).MustHaveHappenedOnceExactly());
        }

        [Fact]
        public async Task HandleAsync_GivenObligationSchemes_SchemesShouldOrdered()
        {
            //arrange
            var schemes = TestFixture.CreateMany<Scheme>().ToList();
            var request = TestFixture.Create<GetSchemesWithObligationOrEvidence>();

            A.CallTo(() => obligationDataAccess.GetSchemesWithObligationOrEvidence(A<int>._)).Returns(schemes);

            //act
            var result = await handler.HandleAsync(request);

            //assert
            result.Should().BeInAscendingOrder(r => r.SchemeName);
        }

        [Fact]
        public async Task HandleAsync_GivenObligationSchemes_SchemesShouldBeReturned()
        {
            //arrange
            var schemes = TestFixture.CreateMany<Scheme>(2).ToList();
            var schemeData = TestFixture.CreateMany<SchemeData>(2).ToList();
            var request = TestFixture.Create<GetSchemesWithObligationOrEvidence>();

            A.CallTo(() => schemeMap.Map(A<Scheme>._)).ReturnsNextFromSequence(schemeData.ToArray());
            A.CallTo(() => obligationDataAccess.GetSchemesWithObligationOrEvidence(A<int>._)).Returns(schemes);

            //act
            var result = await handler.HandleAsync(request);

            //assert
            result.Should().BeEquivalentTo(schemeData);
        }
    }
}
