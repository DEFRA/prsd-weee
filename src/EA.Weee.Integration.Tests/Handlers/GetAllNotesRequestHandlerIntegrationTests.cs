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

    public class GetAllNotesRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIGetAListOfEvidenceNoteDataAsNotesWithCorrectStatusAndType : GetAllNotesRequestHandlerTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var complianceYear = fixture.Create<int>();

                var evidenceWithApprovedStatus = EvidenceNoteDbSetup.Init()
                 .With(n =>
                 {
                     n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                     n.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString(), SystemTime.UtcNow);
                 })
                 .WithComplianceYear(complianceYear)
                 .Create();

                var evidenceWithSubmittedStatus = EvidenceNoteDbSetup.Init()
                 .With(n =>
                 {
                     n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                 })
                 .WithComplianceYear(complianceYear)
                 .Create();

                var evidenceWithReturnedStatus = EvidenceNoteDbSetup.Init()
                .With(n =>
                {
                    n.UpdateStatus(NoteStatusDomain.Returned, UserId.ToString(), SystemTime.UtcNow);
                })
                .WithComplianceYear(complianceYear)
                .Create();

                var evidenceWithRejectedStatus = EvidenceNoteDbSetup.Init()
                  .With(n =>
                  {
                      n.UpdateStatus(NoteStatusDomain.Rejected, UserId.ToString(), SystemTime.UtcNow);
                  })
                  .WithComplianceYear(complianceYear)
                  .Create();

                var evidenceWithVoidStatus = EvidenceNoteDbSetup.Init()
                 .With(n =>
                 {
                     n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                     n.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString(), SystemTime.UtcNow);
                     n.UpdateStatus(NoteStatusDomain.Void, UserId.ToString(), SystemTime.UtcNow);
                 })
                 .WithComplianceYear(complianceYear)
                 .Create();

                notesSet.Add(evidenceWithApprovedStatus);
                notesSet.Add(evidenceWithSubmittedStatus);
                notesSet.Add(evidenceWithReturnedStatus);
                notesSet.Add(evidenceWithRejectedStatus);
                notesSet.Add(evidenceWithVoidStatus);

                request = new GetAllNotesInternal(noteTypeFilter, allowedStatuses, complianceYear, 1, int.MaxValue);
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
                evidenceNoteData.Results.Should().HaveCount(notesSet.Count);
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
        public class WhenIGetAListOfEvidenceNoteDataAsNotesWithNotAllowedStatusAndType : GetAllNotesRequestHandlerTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var evidenceWithDraftStatus1 = EvidenceNoteDbSetup.Init()
                 .Create();

                var evidenceWithDraftStatus2 = EvidenceNoteDbSetup.Init()
                .Create();

                notesSet.Add(evidenceWithDraftStatus1);
                notesSet.Add(evidenceWithDraftStatus2);

                request = new GetAllNotesInternal(noteTypeFilterForTransferNote, notAllowedStatuses, SystemTime.UtcNow.Year, 1, int.MaxValue);
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
            protected static List<NoteType> noteTypeFilterForTransferNote;
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
                noteTypeFilter = new List<NoteType> { NoteType.Evidence };
                noteTypeFilterForTransferNote = new List<NoteType> { NoteType.Transfer };
                handler = Container.Resolve<IRequestHandler<GetAllNotesInternal, EvidenceNoteSearchDataResult>>();
            }
        }
    }
}
