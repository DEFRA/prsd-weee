namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence
{
    using AutoFixture;
    using DataAccess.DataAccess;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.RequestHandlers.AatfEvidence;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfEvidence;
    using FakeItEasy;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Core.AatfEvidence;
    using DataAccess.StoredProcedure;
    using FluentAssertions;
    using Weee.Tests.Core;
    using Xunit;
    using NoteStatus = Domain.Evidence.NoteStatus;

    public class GetAatfSummaryRequestHandlerTests
    {
        private GetAatfSummaryRequestHandler handler;
        private readonly Fixture fixture;
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IEvidenceDataAccess noteDataAccess;
        private readonly IMapper mapper;
        private readonly IEvidenceStoredProcedures evidenceStoredProcedures;
        private readonly GetAatfSummaryRequest request;

        public GetAatfSummaryRequestHandlerTests()
        {
            fixture = new Fixture();
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            noteDataAccess = A.Fake<IEvidenceDataAccess>();
            mapper = A.Fake<IMapper>();
            evidenceStoredProcedures = A.Fake<IEvidenceStoredProcedures>();

            request = new GetAatfSummaryRequest(fixture.Create<Guid>(), 2022);  //TODO: check this

            handler = new GetAatfSummaryRequestHandler(weeeAuthorization,
                noteDataAccess,
                mapper,
                evidenceStoredProcedures);
        }

        [Fact]
        public async Task HandleAsync_GivenNoExternalAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            handler = new GetAatfSummaryRequestHandler(authorization,
                noteDataAccess,
                mapper,
                evidenceStoredProcedures);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(request));

            //assert
            result.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenAatfHasNoOrganisationAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyAatfAccess().Build();

            handler = new GetAatfSummaryRequestHandler(authorization,
                noteDataAccess,
                mapper,
                evidenceStoredProcedures);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(request));

            //assert
            result.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ShouldCheckExternalAccess()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => weeeAuthorization.EnsureCanAccessExternalArea()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ShouldCheckOrganisationAccess()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => weeeAuthorization.EnsureAatfHasOrganisationAccess(request.AatfId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_SummaryTotalsShouldBeRetrieved()
        {
            //arrange
            short currentYear = (short)DateTime.Now.Year;

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => evidenceStoredProcedures.GetAatfEvidenceSummaryTotals(request.AatfId, currentYear))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ApprovedNoteTotalsShouldBeRetrieved()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => noteDataAccess.GetNoteCountByStatusAndAatf(NoteStatus.Approved, request.AatfId))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_SubmittedNoteTotalsShouldBeRetrieved()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => noteDataAccess.GetNoteCountByStatusAndAatf(NoteStatus.Submitted, request.AatfId))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_DraftNoteTotalsShouldBeRetrieved()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => noteDataAccess.GetNoteCountByStatusAndAatf(NoteStatus.Draft, request.AatfId))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestAndSummaryTotals_TotalsMapperShouldBeCalled()
        {
            //arrange
            var totalsData = fixture.CreateMany<AatfEvidenceSummaryTotalsData>().ToList();
            short currentYear = (short)DateTime.Now.Year;

            A.CallTo(() => evidenceStoredProcedures.GetAatfEvidenceSummaryTotals(request.AatfId, currentYear))
                .Returns(totalsData);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => mapper.Map<List<AatfEvidenceSummaryTotalsData>, List<EvidenceSummaryTonnageData>>(totalsData)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestAndTotals_AatfEvidenceSummaryDataShouldBeReturned()
        {
            //arrange
            var tonnageData = fixture.CreateMany<EvidenceSummaryTonnageData>().ToList();
            var approvedNotes = fixture.Create<int>();
            var submittedNotes = fixture.Create<int>();
            var draftNotes = fixture.Create<int>();

            A.CallTo(() =>
                mapper.Map<List<AatfEvidenceSummaryTotalsData>, List<EvidenceSummaryTonnageData>>(
                    A<List<AatfEvidenceSummaryTotalsData>>._)).Returns(tonnageData);

            A.CallTo(() => noteDataAccess.GetNoteCountByStatusAndAatf(A<NoteStatus>._, A<Guid>._)).ReturnsNextFromSequence(approvedNotes, submittedNotes, draftNotes);

            //act
            var result = await handler.HandleAsync(request);

            //assert
            result.EvidenceCategoryTotals.Should().BeEquivalentTo(tonnageData);
            result.NumberOfApprovedNotes.Should().Be(approvedNotes);
            result.NumberOfSubmittedNotes.Should().Be(submittedNotes);
            result.NumberOfDraftNotes.Should().Be(draftNotes);
        }
    }
}
