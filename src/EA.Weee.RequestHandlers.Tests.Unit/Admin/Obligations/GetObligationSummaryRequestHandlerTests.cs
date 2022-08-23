namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Obligations
{
    using AutoFixture;
    using Core.Admin.Obligation;
    using DataAccess.StoredProcedure;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.RequestHandlers.Shared;
    using EA.Weee.Requests.Shared;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using RequestHandlers.Security;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Weee.Tests.Core;
    using Xunit;

    public class GetObligationSummaryRequestHandlerTests : SimpleUnitTestBase
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IMapper mapper;
        private readonly IEvidenceStoredProcedures evidenceStoredProcedures;
        private readonly IOrganisationDataAccess organisationDataAccess;
        private readonly GetObligationSummaryRequestHandler handler;
        private readonly GetObligationSummaryRequest request;

        public GetObligationSummaryRequestHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            mapper = A.Fake<IMapper>();
            evidenceStoredProcedures = A.Fake<IEvidenceStoredProcedures>();
            organisationDataAccess = A.Fake<IOrganisationDataAccess>();

            request = new GetObligationSummaryRequest(TestFixture.Create<Guid>(), TestFixture.Create<int>(), true);

            handler = new GetObligationSummaryRequestHandler(authorization, mapper, evidenceStoredProcedures, organisationDataAccess);
        }

        [Fact]
        public async Task HandleAsync_GivenInternalAccessIsSetToTrue_NoInternalAccess_ThrowsSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            var handler = new GetObligationSummaryRequestHandler(authorization, mapper, evidenceStoredProcedures, organisationDataAccess);

            //act
            var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(request));

            //assert
            exception.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenInternalAccessIsSetToFalse_NoExternalAccess_ThrowsSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();
            var request = new GetObligationSummaryRequest(TestFixture.Create<Guid>(), TestFixture.Create<int>(), false);
            var handler = new GetObligationSummaryRequestHandler(authorization, mapper, evidenceStoredProcedures, organisationDataAccess);

            //act
            var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(request));

            //assert
            exception.Should().BeOfType<SecurityException>();
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
        public async Task HandleAsync_ExternalAccess_ShouldBeChecked()
        {
            // arrange 
            var request = new GetObligationSummaryRequest(TestFixture.Create<Guid>(), TestFixture.Create<int>(), false);

            //act
            await handler.HandleAsync(request);

            //arrange
            A.CallTo(() => authorization.EnsureCanAccessExternalArea()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenNoOrganisationAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyOrganisationAccess().Build();
            var request = new GetObligationSummaryRequest(TestFixture.Create<Guid>(), TestFixture.Create<int>(), false);
            var handler = new GetObligationSummaryRequestHandler(authorization, mapper, evidenceStoredProcedures, organisationDataAccess);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(request));

            //assert
            result.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_EnsureOrganisationAccess_ShouldBeChecked()
        {
            // arrange 
            var organisationId = TestFixture.Create<Guid>();
            var request = new GetObligationSummaryRequest(TestFixture.Create<Guid>(), TestFixture.Create<int>(), false, organisationId);

            //act
            await handler.HandleAsync(request);

            //arrange
            A.CallTo(() => authorization.EnsureOrganisationAccess(organisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ObligationSummaryDataShouldBeRetrieved()
        {
            //act
            await handler.HandleAsync(request);

            //arrange
            A.CallTo(() =>
                    evidenceStoredProcedures.GetObligationEvidenceSummaryTotals(request.SchemeId,
                        request.ComplianceYear)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestAndObligationData_ObligationDataShouldBeMapped()
        {
            //arrange
            var summaryData = TestFixture.CreateMany<ObligationEvidenceSummaryTotalsData>().ToList();

            A.CallTo(() => evidenceStoredProcedures.GetObligationEvidenceSummaryTotals(A<Guid>._, A<int>._))
                .Returns(summaryData);

            //act
            await handler.HandleAsync(request);

            //arrange
            A.CallTo(() => 
                    mapper.Map<List<ObligationEvidenceSummaryTotalsData>, ObligationEvidenceSummaryData>(summaryData)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestAndMappedData_MappedDataShouldBeReturned()
        {
            //arrange
            var mappedData = TestFixture.Create<ObligationEvidenceSummaryData>();

            A.CallTo(() =>
                mapper.Map<List<ObligationEvidenceSummaryTotalsData>, ObligationEvidenceSummaryData>(
                    A<List<ObligationEvidenceSummaryTotalsData>>._)).Returns(mappedData);

            //act
            var result = await handler.HandleAsync(request);

            //arrange
            result.Should().Be(mappedData);
        }
    }
}
