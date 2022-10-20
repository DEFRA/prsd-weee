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
    using FluentAssertions;
    using NUnit.Specifications;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using Requests.AatfEvidence;
    using NoteStatus = Domain.Evidence.NoteStatus;

    public class GetEvidenceNotesSelectedForTransferRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIGetNotesSelectedToTransferForAnOrganisation : GetEvidenceNotesSelectedForTransferRequestHandlerIntegrationTestBase
        {
            private static List<int> categories;

            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(organisation.Id).Create();

                var recipientOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation.Id).Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, recipientOrganisation.Id).Create();

                // note to be included
                var categories1 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 1, null)
                };
                notesSetToBeIncluded.Add(EvidenceNoteDbSetup.Init()
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Approved, UserId.ToString())
                    .WithTonnages(categories1).WithRecipient(recipientOrganisation.Id).Create());

                // returned note should note should only include the requested category
                var categories2 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 10, null),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 5, 1),
                    new NoteTonnage(WeeeCategory.DisplayEquipment, 2, null)
                };
                var evidenceNoteWithTransfers = EvidenceNoteDbSetup.Init()
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Approved, UserId.ToString())
                    .WithTonnages(categories2).WithRecipient(recipientOrganisation.Id).Create();

                notesSetToBeIncluded.Add(evidenceNoteWithTransfers);

                var transferNoteRecipient = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(transferNoteRecipient.Id).Create();

                var transferTonnages1 = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(
                        evidenceNoteWithTransfers.NoteTonnage.First(c =>
                            c.CategoryId.ToInt() == WeeeCategory.AutomaticDispensers.ToInt()).Id, 20, 10)
                };

                // this transfer should not affect the available amount as the note is rejected
                TransferEvidenceNoteDbSetup.Init()
                    .WithOrganisation(recipientOrganisation.Id)
                    .WithRecipient(transferNoteRecipient.Id)
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Rejected, UserId.ToString())
                    .WithTonnages(transferTonnages1)
                    .Create();

                // note to not be included not in note list
                var categories3 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 3, null)
                };
                notesSetToNotBeIncluded.Add(EvidenceNoteDbSetup.Init()
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Approved, UserId.ToString())
                    .WithTonnages(categories3).WithRecipient(recipientOrganisation.Id).Create());

                categories = new List<int>()
                {
                    Core.DataReturns.WeeeCategory.AutomaticDispensers.ToInt(),
                    Core.DataReturns.WeeeCategory.GasDischargeLampsAndLedLightSources.ToInt()
                };

                request = new GetEvidenceNotesSelectedForTransferRequest(recipientOrganisation.Id,
                    new List<Guid>(notesSetToBeIncluded.Select(n => n.Id)), categories);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldHaveReturnedCorrectEvidenceNotes = () =>
            {
                result.Results.Should().HaveCount(2);
                result.NoteCount.Should().Be(2);
                foreach (var evidenceNoteData in result.Results)
                {
                    notesSetToBeIncluded.First(n => n.Id.Equals(evidenceNoteData.Id)).Should().NotBeNull();

                    var refreshedNote = Query.GetEvidenceNoteById(evidenceNoteData.Id);

                    evidenceNoteData.ShouldMapToNote(refreshedNote);

                    notesSetToNotBeIncluded.FirstOrDefault(n => n.Id == evidenceNoteData.Id).Should().BeNull();
                }

                // first note has null automatic dispensers so this should not be returned as tonnage
                var note1 = result.Results.First(r => r.Id.Equals(notesSetToBeIncluded.ElementAt(0).Id));
                note1.EvidenceTonnageData.Count.Should().Be(1);
                note1.EvidenceTonnageData.ElementAt(0).Received.Should().Be(1);
                note1.EvidenceTonnageData.ElementAt(0).Reused.Should().BeNull();
                note1.EvidenceTonnageData.ElementAt(0).Id.Should().NotBe(Guid.Empty);
                note1.EvidenceTonnageData.ElementAt(0).CategoryId.Should().Be(Core.DataReturns.WeeeCategory.AutomaticDispensers);
                note1.TotalReceivedAvailable.Should().Be(1);

                // second note has category DisplayEquipment that is not in the category list so also shouldnt be in the tonnage data
                var note2 = result.Results.First(r => r.Id.Equals(notesSetToBeIncluded.ElementAt(1).Id));
                note2.EvidenceTonnageData.Count.Should().Be(2);
                note2.TotalReceivedAvailable.Should().Be(15);

                var categoryTonnage = note2.EvidenceTonnageData.First(e =>
                    e.CategoryId.Equals(Core.DataReturns.WeeeCategory.GasDischargeLampsAndLedLightSources));
                categoryTonnage.Received.Should().Be(5);
                categoryTonnage.Reused.Should().Be(1);
                categoryTonnage.Id.Should().NotBe(Guid.Empty);
                categoryTonnage = note2.EvidenceTonnageData.First(e =>
                    e.CategoryId.Equals(Core.DataReturns.WeeeCategory.AutomaticDispensers));
                categoryTonnage.Received.Should().Be(10);
                categoryTonnage.Reused.Should().BeNull();
                categoryTonnage.Id.Should().NotBe(Guid.Empty);
            };
        }

        [Component]
        public class WhenIGetNotesToTransferWhereUserIsNotAuthorised : GetEvidenceNotesSelectedForTransferRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                note = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .Create();

                request = new GetEvidenceNotesSelectedForTransferRequest(note.OrganisationId, new List<Guid>() { Guid.NewGuid() }, new List<int>() { 1 });
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => handler.HandleAsync(request));
            };

            private readonly It shouldHaveCaughtArgumentException = ShouldThrowException<SecurityException>;
        }

        public class GetEvidenceNotesSelectedForTransferRequestHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<GetEvidenceNotesSelectedForTransferRequest, EvidenceNoteSearchDataResult> handler;
            protected static Organisation organisation;
            protected static GetEvidenceNotesSelectedForTransferRequest request;
            protected static EvidenceNoteSearchDataResult result;
            protected static Note note;
            protected static Fixture fixture;
            protected static List<Note> notesSetToBeIncluded;
            protected static List<Note> notesSetToNotBeIncluded;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                    .WithExternalUserAccess();

                notesSetToBeIncluded = new List<Note>();
                notesSetToNotBeIncluded = new List<Note>();

                fixture = new Fixture();
                handler = Container.Resolve<IRequestHandler<GetEvidenceNotesSelectedForTransferRequest, EvidenceNoteSearchDataResult>>();

                return setup;
            }
        }
    }
}