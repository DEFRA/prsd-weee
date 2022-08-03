namespace EA.Weee.Integration.Tests.Handlers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Autofac;
    using AutoFixture;
    using Base;
    using Builders;
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Domain.Evidence;
    using EA.Weee.Requests.Admin;
    using FluentAssertions;
    using NUnit.Specifications;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using NoteStatus = Core.AatfEvidence.NoteStatus;
    using NoteStatusDomain = Domain.Evidence.NoteStatus;
    using NoteType = Core.AatfEvidence.NoteType;

    public class GetAllNotesRequestHandlerForTransfersIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIGetAListOfEvidenceNoteDataAsTransferNotesWithCorrectStatusAndType : GetAllNotesRequestHandlerTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var complianceYear = fixture.Create<int>();

                var noteRejectedStatus = TransferEvidenceNoteDbSetup.Init()
                    .WithStatus(NoteStatusDomain.Submitted, UserId.ToString())
                    .WithStatus(NoteStatusDomain.Rejected, UserId.ToString(), "rejected")
                    .WithComplianceYear(complianceYear)
                    .Create();

                var noteApprovedStatus = TransferEvidenceNoteDbSetup.Init()
                 .With(n =>
                 {
                     n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                     n.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString(), SystemTime.UtcNow);
                 })
                 .WithComplianceYear(complianceYear)
                 .Create();

                var notehSubmittedStatus = TransferEvidenceNoteDbSetup.Init()
                 .With(n =>
                 {
                     n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                 })
                 .WithComplianceYear(complianceYear)
                 .Create();

                var noteReturnedStatus = TransferEvidenceNoteDbSetup.Init()
                .With(n =>
                {
                    n.UpdateStatus(NoteStatusDomain.Returned, UserId.ToString(), SystemTime.UtcNow);
                })
                .WithComplianceYear(complianceYear)
                .Create();

                var noteVoidStatus = TransferEvidenceNoteDbSetup.Init()
                 .With(n =>
                 {
                     n.UpdateStatus(NoteStatusDomain.Void, UserId.ToString(), SystemTime.UtcNow);
                 })
                 .WithComplianceYear(complianceYear)
                 .Create();

                notesSet.Add(noteApprovedStatus);
                notesSet.Add(notehSubmittedStatus);
                notesSet.Add(noteRejectedStatus);
                notesSet.Add(noteReturnedStatus);
                notesSet.Add(noteVoidStatus);

                request = new GetAllNotesInternal(noteTypeFilter, allowedStatuses, complianceYear);
            };

            private readonly Because of = () =>
            {
                evidenceNoteData = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldReturnListOfEvidenceNotes = () =>
            {
                evidenceNoteData.Should().NotBeNull();
            };

            private readonly It shouldHaveExpectedResultsCountToBeSetOfNotes = () =>
            {
                evidenceNoteData.Results.Should().HaveSameCount(notesSet);
                evidenceNoteData.NoteCount.Should().Be(notesSet.Count);
            };

            private readonly It shouldHaveExpectedData = () =>
            {
                foreach (var note1 in notesSet)
                {
                    var evidenceNote = evidenceNoteData.Results.ToList().Exists(n => n.Id == note1.Id);
                    evidenceNote.Should().BeTrue();
                }
            };
        }

        [Component]
        public class WhenIGetAListOfEvidenceNoteDataAsTransferNotesWithNotAllowedStatusAndType : GetAllNotesRequestHandlerTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var noteWithDraftStatus1 = TransferEvidenceNoteDbSetup.Init()
                 .Create();

                var noteWithDraftStatus2 = TransferEvidenceNoteDbSetup.Init()
                .Create();

                notesSet.Add(noteWithDraftStatus1);
                notesSet.Add(noteWithDraftStatus2);

                request = new GetAllNotesInternal(noteTypeFilterForEvidenceNote, notAllowedStatuses, SystemTime.UtcNow.Year);
            };

            private readonly Because of = () =>
            {
                evidenceNoteData = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldReturnEmptyListOfEvidenceNotes = () =>
            {
                evidenceNoteData.Results.Should().BeNullOrEmpty();
            };

            private readonly It shouldHaveExpectedResultsCountToBeSetOfNotes = () =>
            {
                evidenceNoteData.Results.Should().HaveCount(0);
                evidenceNoteData.NoteCount.Should().Be(0);
            };
        }

        public class GetAllNotesRequestHandlerTestBase : WeeeContextSpecification
        {
            protected static EvidenceNoteSearchDataResult evidenceNoteData;
            protected static List<Note> notesSet;
            protected static List<NoteStatus> allowedStatuses;
            protected static List<NoteStatus> notAllowedStatuses;
            protected static List<NoteType> noteTypeFilter;
            protected static List<NoteType> noteTypeFilterForEvidenceNote;
            protected static IRequestHandler<GetAllNotesInternal, EvidenceNoteSearchDataResult> handler;
            protected static Fixture fixture;
            protected static GetAllNotesInternal request;

            public static void LocalSetup()
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings(resetDb: true)
                    .WithInternalUserAccess();

                var authority = Query.GetEaCompetentAuthority();
                var role = Query.GetInternalUserRole();

                if (!Query.CompetentAuthorityUserExists(UserId.ToString(), role.Id))
                {
                    CompetentAuthorityUserDbSetup.Init().WithUserIdAndAuthorityAndRole(UserId.ToString(), authority.Id, role.Id)
                        .Create();
                }

                fixture = new Fixture();
                notesSet = new List<Note>();
                allowedStatuses = new List<NoteStatus> { NoteStatus.Approved, NoteStatus.Rejected, NoteStatus.Submitted, NoteStatus.Void, NoteStatus.Returned };
                notAllowedStatuses = new List<NoteStatus> { NoteStatus.Draft };
                noteTypeFilter = new List<NoteType> { NoteType.Transfer };
                noteTypeFilterForEvidenceNote = new List<NoteType> { NoteType.Evidence };
                handler = Container.Resolve<IRequestHandler<GetAllNotesInternal, EvidenceNoteSearchDataResult>>();
            }
        }
    }
}
