﻿namespace EA.Weee.Integration.Tests.Handlers
{
    using System.Collections.Generic;
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

                var evidenceWithApprovedStatus = EvidenceNoteDbSetup.Init()
                 .With(n =>
                 {
                     n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                     n.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString(), SystemTime.UtcNow);
                 })
                 .Create();

                var evidenceWithSubmittedStatus = EvidenceNoteDbSetup.Init()
                 .With(n =>
                 {
                     n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                 })
                 .Create();

                var evidenceWithReturnedStatus = EvidenceNoteDbSetup.Init()
                .With(n =>
                {
                    n.UpdateStatus(NoteStatusDomain.Returned, UserId.ToString(), SystemTime.UtcNow);
                })
                .Create();

                var evidenceWithRejectedStatus = EvidenceNoteDbSetup.Init()
                  .With(n =>
                  {
                      n.UpdateStatus(NoteStatusDomain.Rejected, UserId.ToString(), SystemTime.UtcNow);
                  })
                  .Create();

                var evidenceWithVoidStatus = EvidenceNoteDbSetup.Init()
                 .With(n =>
                 {
                     n.UpdateStatus(NoteStatusDomain.Void, UserId.ToString(), SystemTime.UtcNow);
                 })
                 .Create();

                notesSet.Add(evidenceWithApprovedStatus);
                notesSet.Add(evidenceWithSubmittedStatus);
                notesSet.Add(evidenceWithReturnedStatus);
                notesSet.Add(evidenceWithRejectedStatus);
                notesSet.Add(evidenceWithVoidStatus);
            };

            private readonly Because of = () =>
            {
                evidenceNoteData = Task.Run(async () => await handler.HandleAsync(new GetAllNotes(noteTypeFilter, allowedStatuses))).Result;
            };

            private readonly It shouldReturnListOfEvidenceNotes = () =>
            {
                evidenceNoteData.Should().NotBeNull();
            };

            private readonly It shouldHaveExpectedResultsCountToBeSetOfNotes = () =>
            {
                evidenceNoteData.Should().HaveSameCount(notesSet);
            };

            private readonly It shouldHaveExpectedData = () =>
            {
                foreach (var note1 in notesSet)
                {
                    var evidenceNote = evidenceNoteData.Exists(n => n.Id == note1.Id);
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
            };

            private readonly Because of = () =>
            {
                evidenceNoteData = Task.Run(async () => await handler.HandleAsync(new GetAllNotes(noteTypeFilterForTransferNote, notAllowedStatuses))).Result;
            };

            private readonly It shouldReturnEmptyListOfEvidenceNotes = () =>
            {
                evidenceNoteData.Should().BeNullOrEmpty();
            };

            private readonly It shouldHaveExpectedResultsCountToBeSetOfNotes = () =>
            {
                evidenceNoteData.Should().HaveCount(0);
            };
        }

        public class GetAllNotesRequestHandlerTestBase : WeeeContextSpecification
        {
            protected static List<EvidenceNoteData> evidenceNoteData;
            protected static List<Note> notesSet;
            protected static List<NoteStatus> allowedStatuses;
            protected static List<NoteStatus> notAllowedStatuses;
            protected static List<NoteType> noteTypeFilter;
            protected static List<NoteType> noteTypeFilterForTransferNote;
            protected static IRequestHandler<GetAllNotes, List<EvidenceNoteData>> handler;
            protected static Fixture fixture;

            public static void LocalSetup()
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings(resetDb: true)
                    .WithInternalAdminUserAccess();

                var authority = Query.GetEaCompetentAuthority();
                var role = Query.GetInternalUserRole();

                if (!Query.CompetentAuthorityUserExists(UserId.ToString()))
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
                handler = Container.Resolve<IRequestHandler<GetAllNotes, List<EvidenceNoteData>>>();
            }
        }
    }
}
