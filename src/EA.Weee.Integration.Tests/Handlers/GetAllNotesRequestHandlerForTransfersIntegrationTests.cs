namespace EA.Weee.Integration.Tests.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Autofac;
    using AutoFixture;
    using Base;
    using Builders;
    using Core.Shared;
    using Domain.Organisation;
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
    using WasteType = Core.AatfEvidence.WasteType;

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
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .WithStatus(NoteStatusDomain.Submitted, UserId.ToString())
                    .WithStatus(NoteStatusDomain.Rejected, UserId.ToString(), "rejected")
                    .WithComplianceYear(complianceYear)
                    .Create();

                var noteApprovedStatus = TransferEvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                        n.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString(), SystemTime.UtcNow);
                    })
                    .WithComplianceYear(complianceYear)
                    .Create();

                var notehSubmittedStatus = TransferEvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                    })
                    .WithComplianceYear(complianceYear)
                    .Create();

                var noteReturnedStatus = TransferEvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                        n.UpdateStatus(NoteStatusDomain.Returned, UserId.ToString(), SystemTime.UtcNow);
                    })
                    .WithComplianceYear(complianceYear)
                    .Create();

                var noteVoidStatus = TransferEvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                        n.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString(), SystemTime.UtcNow);
                        n.UpdateStatus(NoteStatusDomain.Void, UserId.ToString(), SystemTime.UtcNow);
                    })
                    .WithComplianceYear(complianceYear)
                    .Create();

                notesSet.Add(noteApprovedStatus);
                notesSet.Add(notehSubmittedStatus);
                notesSet.Add(noteRejectedStatus);
                notesSet.Add(noteReturnedStatus);
                notesSet.Add(noteVoidStatus);

                request = new GetAllNotesInternal(noteTypeFilter, allowedStatuses, complianceYear, 1, int.MaxValue, 
                    null, null, null, null, null, null, null, null);
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
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .Create();

                var noteWithDraftStatus2 = TransferEvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .Create();

                notesSet.Add(noteWithDraftStatus1);
                notesSet.Add(noteWithDraftStatus2);

                request = new GetAllNotesInternal(noteTypeFilterForEvidenceNote, notAllowedStatuses, SystemTime.UtcNow.Year, 1, int.MaxValue,
                    null, null, null, null, null, null, null, null);
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

        [Component]
        public class WhenIGetAListOfEvidenceNoteDataAsTransferNotesWithTransferOrganisation : GetAllNotesRequestHandlerTestBase
        {
            private static Organisation transferOrganisation;

            private readonly Establish context = () =>
            {
                LocalSetup(false);

                transferOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(transferOrganisation.Id).Create();

                var recipientScheme = SchemeDbSetup.Init().WithNewOrganisation().Create();

                var note1 = TransferEvidenceNoteDbSetup.Init()
                    .WithOrganisation(transferOrganisation.Id)
                    .WithRecipient(recipientScheme.OrganisationId)
                    .Create();

                var noMatchingTransferOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(noMatchingTransferOrganisation.Id).Create();

                TransferEvidenceNoteDbSetup.Init()
                    .WithOrganisation(noMatchingTransferOrganisation.Id)
                    .WithRecipient(recipientScheme.OrganisationId)
                    .Create();

                var note2 = TransferEvidenceNoteDbSetup.Init()
                    .WithOrganisation(transferOrganisation.Id)
                    .WithRecipient(recipientScheme.OrganisationId)
                    .Create();

                notesSet.Add(note1);
                notesSet.Add(note2);
                
                request = new GetAllNotesInternal(new List<NoteType>(), 
                    new List<NoteStatus>()
                    {
                        NoteStatus.Draft
                    }, SystemTime.UtcNow.Year, 1, int.MaxValue,
                    null, null, null, null, null, null, null, transferOrganisation.Id);
            };

            private readonly Because of = () =>
            {
                evidenceNoteData = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldReturnCorrectNotes = () =>
            {
                evidenceNoteData.NoteCount.Should().Be(2);
                notesSet.All(e => evidenceNoteData.Results.Select(n => n.Id).Contains(e.Id)).Should().BeTrue();
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
            protected static DateTime? startDateSubmittedFilter;
            protected static DateTime? endDateSubmittedFilter;
            protected static Guid? recipientIdFilter;
            protected static NoteStatus? noteStatusFilter;
            protected static WasteType? obligationTypeFilter;
            protected static Guid? submittedAatfIdFilter;

            public static void LocalSetup(bool clearDb = true)
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings(resetDb: clearDb)
                    .WithInternalUserAccess();

                Query.SetupUserWithRole(UserId.ToString(), "Standard", CompetentAuthority.England);

                fixture = new Fixture();
                notesSet = new List<Note>();
                allowedStatuses = new List<NoteStatus> { NoteStatus.Approved, NoteStatus.Rejected, NoteStatus.Submitted, NoteStatus.Void, NoteStatus.Returned };
                notAllowedStatuses = new List<NoteStatus> { NoteStatus.Draft };
                noteTypeFilter = new List<NoteType> { NoteType.Transfer };
                noteTypeFilterForEvidenceNote = new List<NoteType> { NoteType.Evidence };
                handler = Container.Resolve<IRequestHandler<GetAllNotesInternal, EvidenceNoteSearchDataResult>>();
                startDateSubmittedFilter = DateTime.Now.AddDays(-10);
                endDateSubmittedFilter = DateTime.Now.AddDays(10);
                recipientIdFilter = fixture.Create<Guid>();
                noteStatusFilter = NoteStatus.Approved;
                obligationTypeFilter = WasteType.Household;
                submittedAatfIdFilter = fixture.Create<Guid>();
            }
        }
    }
}
