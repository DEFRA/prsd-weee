namespace EA.Weee.Integration.Tests.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Autofac;
    using AutoFixture;
    using Base;
    using Builders;
    using Core.AatfEvidence;
    using Core.Helpers;
    using Domain.Evidence;
    using Domain.Lookup;
    using Domain.Organisation;
    using EA.Weee.Domain.Scheme;
    using FluentAssertions;
    using NUnit.Specifications;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using Requests.AatfEvidence;
    using NoteStatus = Domain.Evidence.NoteStatus;
    using NoteType = Core.AatfEvidence.NoteType;

    public class GetTransferEvidenceNoteForSchemeHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIGetADraftTransferEvidenceNote : GetTransferEvidenceNoteForSchemeHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, organisation.Id).Create();
                SchemeDbSetup.Init().WithOrganisation(organisation.Id).Create();

                var noteTonnages1 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 2, 1)
                };

                var newNote1 = EvidenceNoteDbSetup.Init().WithTonnages(noteTonnages1).Create();

                var noteTonnages2 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 3, 1)
                };

                var newNote2 = EvidenceNoteDbSetup.Init().WithTonnages(noteTonnages2).Create();

                var noteTransferTonnage = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(newNote1.NoteTonnage.ElementAt(0).Id, 1, null),
                    new NoteTransferTonnage(newNote2.NoteTonnage.ElementAt(0).Id, 2, 1)
                };

                transferredNotes.Add(new Tuple<Note, NoteTransferTonnage>(newNote1, noteTransferTonnage.ElementAt(0)));
                transferredNotes.Add(new Tuple<Note, NoteTransferTonnage>(newNote2, noteTransferTonnage.ElementAt(1)));

                note = TransferEvidenceNoteDbSetup.Init().WithTonnages(noteTransferTonnage).WithOrganisation(organisation.Id).Create();

                request = new GetTransferEvidenceNoteForSchemeRequest(note.Id);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                note = Query.GetEvidenceNoteById(note.Id);
            };

            private readonly It shouldHaveReturnedTheTransferEvidenceNote = () =>
            {
                result.Should().NotBeNull();
            };

            private readonly It shouldHaveReturnedTheTransferEvidenceNoteWithExpectedPropertyValues = () =>
            {
                ShouldMapToNote();
                result.Status.Should().Be(EA.Weee.Core.AatfEvidence.NoteStatus.Draft);
            };

            private readonly It shouldHaveReturnedTheTransferEvidenceNoteWithTransferredTonnages = () =>
            {
                result.TransferEvidenceNoteTonnageData.Count.Should().Be(2);
                foreach (var transferredNote in transferredNotes)
                {
                    var transferTonnage =
                        result.TransferEvidenceNoteTonnageData.First(t => t.Reference == transferredNote.Item1.Reference);

                    transferTonnage.Should().NotBeNull();
                    transferTonnage.OriginalAatf.Id.Should().Be(transferredNote.Item1.Aatf.Id);
                    transferTonnage.Type.Should().Be(NoteType.Evidence);
                    transferTonnage.Reference.Should().Be(transferredNote.Item1.Reference);
                    transferTonnage.EvidenceTonnageData.TransferredReceived.Should().Be(transferredNote.Item2.Received);
                    transferTonnage.EvidenceTonnageData.TransferredReused.Should().Be(transferredNote.Item2.Reused);
                    transferTonnage.EvidenceTonnageData.Received.Should().Be(transferredNote.Item1.NoteTonnage.ElementAt(0).Received);
                    transferTonnage.EvidenceTonnageData.Reused.Should().Be(transferredNote.Item1.NoteTonnage.ElementAt(0).Reused);
                }
            };
        }

        [Component]
        public class WhenIGetASubmittedTransferEvidenceNote : GetTransferEvidenceNoteForSchemeHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, organisation.Id).Create();
                SchemeDbSetup.Init().WithOrganisation(organisation.Id).Create();

                var noteTonnages = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 2, 1)
                };

                var newNote = EvidenceNoteDbSetup.Init().WithTonnages(noteTonnages).Create();

                var noteTransferTonnage = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(newNote.NoteTonnage.ElementAt(0).Id, 1, null)
                };

                transferredNotes.Add(new Tuple<Note, NoteTransferTonnage>(newNote, noteTransferTonnage.ElementAt(0)));

                note = TransferEvidenceNoteDbSetup.Init().WithTonnages(noteTransferTonnage)
                    .WithOrganisation(organisation.Id)
                    .WithStatus(NoteStatus.Submitted, UserId.ToString()).Create();

                request = new GetTransferEvidenceNoteForSchemeRequest(note.Id);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                note = Query.GetEvidenceNoteById(note.Id);
            };

            private readonly It shouldHaveReturnedTheTransferEvidenceNote = () =>
            {
                result.Should().NotBeNull();
            };

            private readonly It shouldHaveReturnedTheTransferNoteWithExpectedPropertyValues = () =>
            {
                ShouldMapToNote();
                result.Status.Should().Be(EA.Weee.Core.AatfEvidence.NoteStatus.Submitted);
                //TODO: this to go back in when the mapping is in place (another story)
                //result.SubmittedDate.Should().Be(note.NoteStatusHistory.First(n => n.ToStatus.Equals(NoteStatus.Submitted)).ChangedDate);
            };

            private readonly It shouldHaveReturnedTheTransferEvidenceNoteWithTransferredTonnages = () =>
            {
                result.TransferEvidenceNoteTonnageData.Count.Should().Be(1);
                foreach (var transferredNote in transferredNotes)
                {
                    var transferTonnage =
                        result.TransferEvidenceNoteTonnageData.First(t => t.Reference == transferredNote.Item1.Reference);

                    transferTonnage.Should().NotBeNull();
                    transferTonnage.OriginalAatf.Id.Should().Be(transferredNote.Item1.Aatf.Id);
                    transferTonnage.Type.Should().Be(NoteType.Evidence);
                    transferTonnage.Reference.Should().Be(transferredNote.Item1.Reference);
                    transferTonnage.EvidenceTonnageData.TransferredReceived.Should().Be(transferredNote.Item2.Received);
                    transferTonnage.EvidenceTonnageData.TransferredReused.Should().Be(transferredNote.Item2.Reused);
                    transferTonnage.EvidenceTonnageData.Received.Should().Be(transferredNote.Item1.NoteTonnage.ElementAt(0).Received);
                    transferTonnage.EvidenceTonnageData.Reused.Should().Be(transferredNote.Item1.NoteTonnage.ElementAt(0).Reused);
                }
            };
        }

        [Component]
        public class WhenIGetATransferNoteWhereNoteDoesNotExist : GetTransferEvidenceNoteForSchemeHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                request = new GetTransferEvidenceNoteForSchemeRequest(Guid.NewGuid());
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => handler.HandleAsync(request));
            };

            private readonly It shouldHaveCaughtArgumentNullException = ShouldThrowException<ArgumentNullException>;
        }

        [Component]
        public class WhenIGetANotesWhereUserIsNotAuthorised : GetTransferEvidenceNoteForSchemeHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                note = TransferEvidenceNoteDbSetup.Init().Create();

                request = new GetTransferEvidenceNoteForSchemeRequest(note.Id);
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => handler.HandleAsync(request));
            };

            private readonly It shouldHaveCaughtArgumentException = ShouldThrowException<SecurityException>;
        }

        public class GetTransferEvidenceNoteForSchemeHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<GetTransferEvidenceNoteForSchemeRequest, TransferEvidenceNoteData> handler;
            protected static Organisation organisation;
            protected static GetTransferEvidenceNoteForSchemeRequest request;
            protected static TransferEvidenceNoteData result;
            protected static Scheme scheme;
            protected static Note note;
            protected static Fixture fixture;
            protected static List<Tuple<Note, NoteTransferTonnage>> transferredNotes;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                    .WithExternalUserAccess();

                fixture = new Fixture();
                handler = Container.Resolve<IRequestHandler<GetTransferEvidenceNoteForSchemeRequest, TransferEvidenceNoteData>>();
                transferredNotes = new List<Tuple<Note, NoteTransferTonnage>>();
                return setup;
            }
            protected static void ShouldMapToNote()
            {
                result.Reference.Should().Be(note.Reference);
                result.Status.ToInt().Should().Be(note.Status.Value);
                ((int)result.Type).Should().Be(note.NoteType.Value);
                result.Id.Should().Be(note.Id);
                result.ComplianceYear.Should().Be(note.ComplianceYear);
                result.RecipientOrganisationData.Id.Should().Be(note.Recipient.OrganisationId);
                result.RecipientSchemeData.Id.Should().Be(note.RecipientId);
                result.TransferredOrganisationData.Id.Should().Be(note.OrganisationId);

                var recipientScheme = Query.GetSchemeByOrganisationId(note.OrganisationId);
                result.TransferredSchemeData.Id.Should().Be(recipientScheme.Id);
            }
        }
    }
}
