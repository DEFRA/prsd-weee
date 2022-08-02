namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.ManageEvidenceNotes
{
    using AutoFixture;
    using Core.Helpers;
    using DataAccess.DataAccess;
    using Domain.Evidence;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.RequestHandlers.Admin;
    using EA.Weee.RequestHandlers.Mappings;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;
    using NoteStatus = Core.AatfEvidence.NoteStatus;
    using NoteType = Core.AatfEvidence.NoteType;

    public class GetComplianceYearsFilterHandlerTests : SimpleUnitTestBase
    {
        private GetComplianceYearsFilterHandler handler;
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly GetComplianceYearsFilter request;

        public GetComplianceYearsFilterHandlerTests()
        {
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            evidenceDataAccess = A.Fake<IEvidenceDataAccess>();
            request = new GetComplianceYearsFilter(TestFixture.CreateMany<NoteStatus>().ToList());

            handler = new GetComplianceYearsFilterHandler(weeeAuthorization, evidenceDataAccess);
        }

        [Fact]
        public async Task HandleAsync_GivenNoInternalAccess_ShouldThrowSecurityException()
        {
            //arrange
            var noAccessauthorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            handler = new GetComplianceYearsFilterHandler(noAccessauthorization, evidenceDataAccess);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(TestFixture.Create<GetComplianceYearsFilter>()));

            //assert
            result.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ShouldCallEnsureCanAccessInternalArea()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => weeeAuthorization.EnsureCanAccessInternalArea())
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void HandleAsync_GivenRequest_EvidenceDataAccessShouldBeCalledOnce()
        {
            // act
            await handler.HandleAsync(request);

            var allowedStatuses = request.AllowedStatuses
                .Select(a => a.ToDomainEnumeration<Domain.Evidence.NoteStatus>()).ToList();

            var allowedStatusesIntList = allowedStatuses.Select(v => v.Value).ToList();

            // assert
            A.CallTo(() => evidenceDataAccess.GetComplianceYearsForNotes(A<List<int>>.That.Matches(e =>
                e.SequenceEqual(allowedStatusesIntList)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void HandleAsync_GivenNotesData_ReturnedNotesDataShouldBeMapped()
        {
            // arrange
            var note1 = A.Fake<Note>();
            var complianceYearNote1 = DateTime.Now.Year;

            var note2 = A.Fake<Note>();
            var complianceYearNote2 = DateTime.Now.Year + 1;

            var note3 = A.Fake<Note>();
            var complianceYearNote3 = DateTime.Now.Year - 1;

            var note4 = A.Fake<Note>();
            var complianceYearNote4 = DateTime.Now.Year - 3;

            A.CallTo(() => note1.Reference).Returns(1);
            A.CallTo(() => note1.ComplianceYear).Returns(complianceYearNote1);
            A.CallTo(() => note2.Reference).Returns(2);
            A.CallTo(() => note2.ComplianceYear).Returns(complianceYearNote2);
            A.CallTo(() => note3.Reference).Returns(3);
            A.CallTo(() => note3.ComplianceYear).Returns(complianceYearNote3);
            A.CallTo(() => note3.Reference).Returns(4);
            A.CallTo(() => note3.ComplianceYear).Returns(complianceYearNote4);

            var complianceYearList = new List<int>()
            {
                complianceYearNote2,
                complianceYearNote1,


            };
            var noteData = new EvidenceNoteResults(noteList, noteList.Count);

            A.CallTo(() => evidenceDataAccess.GetComplianceYearsForNotes(A<List<int>>._)).Returns(noteData);

            // act
            await handler.HandleAsync(message);

            // assert
            A.CallTo(() => mapper.Map<ListOfEvidenceNoteDataMap>(A<ListOfNotesMap>.That.Matches(a =>
                a.ListOfNotes.ElementAt(0).Reference.Equals(6) &&
                a.ListOfNotes.ElementAt(1).Reference.Equals(2) &&
                a.ListOfNotes.ElementAt(2).Reference.Equals(4) &&
                a.ListOfNotes.Count.Equals(3) &&
                a.IncludeTonnage == false))).MustHaveHappenedOnceExactly();

            A.CallTo(() => noteDataAccess.GetAllNotes(A<NoteFilter>._)).MustHaveHappenedOnceExactly();
        }
    }
}
