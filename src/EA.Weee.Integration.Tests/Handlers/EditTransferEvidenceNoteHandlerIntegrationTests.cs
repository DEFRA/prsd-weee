﻿namespace EA.Weee.Integration.Tests.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Autofac;
    using Base;
    using Builders;
    using Core.Helpers;
    using Domain.Evidence;
    using Domain.Lookup;
    using Domain.Organisation;
    using FluentAssertions;
    using NUnit.Specifications;
    using NUnit.Specifications.Categories;
    using Prsd.Core;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using Requests.AatfEvidence;
    using Requests.Scheme;
    using NoteStatus = Domain.Evidence.NoteStatus;
    using NoteType = Domain.Evidence.NoteType;
    using WasteType = Domain.Evidence.WasteType;

    public class EditTransferEvidenceNoteRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIEditADraftTransferEvidenceNote : EditTransferEvidenceNoteHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                transferOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(transferOrganisation.Id).Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, transferOrganisation.Id).Create();

                existingRecipientOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(existingRecipientOrganisation.Id).Create();

                var existingTonnagesNote1 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, 2, 1),
                    new NoteTonnage(WeeeCategory.DisplayEquipment, 3, 1),
                    new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 4, 1),
                };

                var note1 = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .WithTonnages(existingTonnagesNote1).Create();

                var existingTonnagesNote2 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 5, 1),
                    new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 10, 1),
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 11, 1),
                };

                var note2 = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .WithTonnages(existingTonnagesNote2).Create();

                var transferTonnage1 =
                    note1.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.DisplayEquipment));
                var transferTonnage2 =
                    note2.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.LargeHouseholdAppliances));
                var transferTonnage3 =
                    note2.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.MonitoringAndControlInstruments));
                var transferTonnage4 =
                    note1.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ConsumerEquipment));
                var transferTonnage5 =
                    note1.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.LargeHouseholdAppliances));
                var transferTonnage6 =
                    note2.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.AutomaticDispensers));

                var existingTransferTonnages = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(transferTonnage1.Id, 3, 1),
                    new NoteTransferTonnage(transferTonnage2.Id, 4, null),
                    new NoteTransferTonnage(transferTonnage3.Id, 5, 1),
                    new NoteTransferTonnage(transferTonnage5.Id, null, null),
                };

                var transferNote = TransferEvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .WithOrganisation(transferOrganisation.Id)
                    .WithTonnages(existingTransferTonnages).Create();

                updatedRecipientOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(updatedRecipientOrganisation.Id).Create();

                transferTonnageValues = new List<TransferTonnageValue>()
                {
                    new TransferTonnageValue(transferTonnage1.Id, WeeeCategory.DisplayEquipment.ToInt(), 1, null, Guid.Empty),
                    new TransferTonnageValue(transferTonnage4.Id, WeeeCategory.ConsumerEquipment.ToInt(), 2, 2, Guid.Empty),
                    new TransferTonnageValue(transferTonnage6.Id, WeeeCategory.AutomaticDispensers.ToInt(), 0, 0, Guid.Empty),
                    new TransferTonnageValue(transferTonnage5.Id, WeeeCategory.LargeHouseholdAppliances.ToInt(), null, null, Guid.Empty),
                };

                request = new EditTransferEvidenceNoteRequest(transferNote.Id, transferNote.OrganisationId, updatedRecipientOrganisation.Id, transferTonnageValues, EA.Weee.Core.AatfEvidence.NoteStatus.Draft);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                note = Query.GetTransferEvidenceNoteById(result);
            };

            private readonly It shouldHaveUpdatedTheTransferEvidenceNoteWithExpectedPropertyValues = () =>
            {
                ShouldMapToNote();
                note.Status.Should().Be(NoteStatus.Draft);
                note.NoteTransferTonnage.Count.Should().Be(4);
                foreach (var transferTonnageValue in transferTonnageValues)
                {
                    var tonnage = note.NoteTransferTonnage.FirstOrDefault(ntt =>
                        ntt.NoteTonnageId.Equals(transferTonnageValue.Id));

                    tonnage.Should().NotBeNull();
                    tonnage.Received.Should().Be(transferTonnageValue.FirstTonnage);
                    tonnage.Reused.Should().Be(transferTonnageValue.SecondTonnage);
                }
            };

            private readonly It shouldHaveUpdatedTransferTonnagesWithExpectedValues = () =>
            {
                note.NoteTransferTonnage.Count.Should().Be(4);
                var transferNoteTonnage = note.NoteTransferTonnage.First(nt =>
                    nt.NoteTonnage.CategoryId.ToInt() == WeeeCategory.DisplayEquipment.ToInt());
                transferNoteTonnage.Received.Should().Be(1);
                transferNoteTonnage.Reused.Should().Be(null);
                transferNoteTonnage = note.NoteTransferTonnage.First(nt =>
                    nt.NoteTonnage.CategoryId.ToInt() == WeeeCategory.ConsumerEquipment.ToInt());
                transferNoteTonnage.Received.Should().Be(2);
                transferNoteTonnage.Reused.Should().Be(2);
                transferNoteTonnage = note.NoteTransferTonnage.First(nt =>
                    nt.NoteTonnage.CategoryId.ToInt() == WeeeCategory.AutomaticDispensers.ToInt());
                transferNoteTonnage.Received.Should().Be(0);
                transferNoteTonnage.Reused.Should().Be(0);
                transferNoteTonnage = note.NoteTransferTonnage.First(nt =>
                    nt.NoteTonnage.CategoryId.ToInt() == WeeeCategory.LargeHouseholdAppliances.ToInt());
                transferNoteTonnage.Received.Should().Be(null);
                transferNoteTonnage.Reused.Should().Be(null);
            };

            private readonly It shouldHaveNotSetApprovedDetails = () =>
            {
                note.ApprovedRecipientAddress.Should().BeNull();
                note.ApprovedRecipientSchemeName.Should().BeNull();
                note.ApprovedTransfererAddress.Should().BeNull();
                note.ApprovedTransfererSchemeName.Should().BeNull();
            };
        }

        [Component]
        public class WhenIEditADraftTransferEvidenceNoteThatIsToBeSubmitted : EditTransferEvidenceNoteHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                transferOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(transferOrganisation.Id).Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, transferOrganisation.Id).Create();

                existingRecipientOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(existingRecipientOrganisation.Id).Create();

                var existingTonnagesNote1 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, 2, 1),
                    new NoteTonnage(WeeeCategory.DisplayEquipment, 3, 1),
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 4, 1),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 5, 1),
                };

                var note1 = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .WithTonnages(existingTonnagesNote1).Create();

                var existingTonnagesNote2 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 5, 1),
                    new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 10, 1),
                    new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 11, 1),
                    new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 12, 1),
                };

                var note2 = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .WithTonnages(existingTonnagesNote2).Create();

                var transferTonnage1 =
                    note1.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.DisplayEquipment));
                var transferTonnage2 =
                    note2.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.LargeHouseholdAppliances));
                var transferTonnage3 =
                    note2.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.MonitoringAndControlInstruments));
                var transferTonnage4 =
                    note1.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ConsumerEquipment));
                var transferTonnage5 =
                    note1.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.PhotovoltaicPanels));
                var transferTonnage6 =
                    note1.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.GasDischargeLampsAndLedLightSources));
                var transferTonnage7 =
                    note2.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ITAndTelecommsEquipment));
                var transferTonnage8 =
                    note2.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ToysLeisureAndSports));

                var existingTransferTonnages = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(transferTonnage1.Id, 3, 1),
                    new NoteTransferTonnage(transferTonnage2.Id, 4, null),
                    new NoteTransferTonnage(transferTonnage3.Id, 5, 1),
                    new NoteTransferTonnage(transferTonnage4.Id, 3, 1),
                    new NoteTransferTonnage(transferTonnage5.Id, null, null),
                    new NoteTransferTonnage(transferTonnage6.Id, 0, 0),
                };

                var transferNote = TransferEvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .WithOrganisation(transferOrganisation.Id)
                    .WithTonnages(existingTransferTonnages).Create();

                updatedRecipientOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(updatedRecipientOrganisation.Id).Create();

                transferTonnageValues = new List<TransferTonnageValue>()
                {
                    new TransferTonnageValue(transferTonnage3.Id, WeeeCategory.MonitoringAndControlInstruments.ToInt(), 10, 5, Guid.Empty),
                    new TransferTonnageValue(transferTonnage1.Id, WeeeCategory.DisplayEquipment.ToInt(), 2, 2, Guid.Empty),
                    new TransferTonnageValue(transferTonnage7.Id, WeeeCategory.ITAndTelecommsEquipment.ToInt(), null, null, Guid.Empty),
                    new TransferTonnageValue(transferTonnage8.Id, WeeeCategory.ToysLeisureAndSports.ToInt(), 0, 0, Guid.Empty),
                };

                request = new EditTransferEvidenceNoteRequest(transferNote.Id, transferNote.OrganisationId, updatedRecipientOrganisation.Id, transferTonnageValues, EA.Weee.Core.AatfEvidence.NoteStatus.Submitted);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                note = Query.GetTransferEvidenceNoteById(result);
            };

            private readonly It shouldHaveUpdatedTheTransferEvidenceNoteWithExpectedPropertyValues = () =>
            {
                ShouldMapToNote();
                note.Status.Should().Be(NoteStatus.Submitted);
                note.NoteTransferTonnage.Count.Should().Be(2);
                foreach (var transferTonnageValue in transferTonnageValues.Where(t => t.FirstTonnage != null && t.FirstTonnage != 0.000M))
                {
                    var tonnage = note.NoteTransferTonnage.FirstOrDefault(ntt =>
                        ntt.NoteTonnageId.Equals(transferTonnageValue.Id));

                    tonnage.Should().NotBeNull();
                    tonnage.Received.Should().Be(transferTonnageValue.FirstTonnage);
                    tonnage.Reused.Should().Be(transferTonnageValue.SecondTonnage);
                }
            };

            private readonly It shouldHaveUpdatedTransferTonnagesWithExpectedValues = () =>
            {
                note.NoteTransferTonnage.Count.Should().Be(2);
                var transferNoteTonnage = note.NoteTransferTonnage.First(nt =>
                    nt.NoteTonnage.CategoryId.ToInt() == WeeeCategory.MonitoringAndControlInstruments.ToInt());
                transferNoteTonnage.Received.Should().Be(10);
                transferNoteTonnage.Reused.Should().Be(5);
                transferNoteTonnage = note.NoteTransferTonnage.First(nt =>
                    nt.NoteTonnage.CategoryId.ToInt() == WeeeCategory.DisplayEquipment.ToInt());
                transferNoteTonnage.Received.Should().Be(2);
                transferNoteTonnage.Reused.Should().Be(2);
            };

            private readonly It shouldHaveStatusHistory = () =>
            {
                var history = Query.GetLatestNoteStatusHistoryForNote(note.Id);

                history.ChangedById.Should().Be(UserId.ToString());
                history.ChangedDate.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromSeconds(10));
                history.FromStatus.Should().Be(NoteStatus.Draft);
                history.ToStatus.Should().Be(NoteStatus.Submitted);
            };

            private readonly It shouldHaveNotSetApprovedDetails = () =>
            {
                note.ApprovedRecipientAddress.Should().BeNull();
                note.ApprovedRecipientSchemeName.Should().BeNull();
                note.ApprovedTransfererAddress.Should().BeNull();
                note.ApprovedTransfererSchemeName.Should().BeNull();
            };

            private readonly It shouldHaveRemovedNullAndZeroTonnages = () =>
            {
                note.NoteTransferTonnage.Count(ntt => ntt.Received == null).Should().Be(0);
                note.NoteTransferTonnage.Count(ntt => ntt.Received == 0.000M).Should().Be(0);
            };
        }

        [Component]
        public class WhenIEditADraftTransferEvidenceNoteAgainstNotesWithTonnageTransfer : EditTransferEvidenceNoteHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                transferOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(transferOrganisation.Id).Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, transferOrganisation.Id).Create();

                existingRecipientOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(existingRecipientOrganisation.Id).Create();

                var existingTonnagesNote1 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, 5, 1)
                };

                var note1 = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .WithTonnages(existingTonnagesNote1).Create();

                var transferTonnage1 =
                    note1.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ConsumerEquipment));

                // create transfer from note 1
                var newTransferNoteTonnage1 = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(transferTonnage1.Id, 2, null)
                };

                TransferEvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(t =>
                {
                    t.UpdateStatus(NoteStatus.Submitted, UserId.ToString(), SystemTime.UtcNow);
                    t.UpdateStatus(NoteStatus.Approved, UserId.ToString(), SystemTime.UtcNow);
                }).WithTonnages(newTransferNoteTonnage1).Create();

                // create another transfer from note 1
                var newTransferNoteTonnage2 = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(transferTonnage1.Id, 2, null)
                };

                TransferEvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(t =>
                {
                    t.UpdateStatus(NoteStatus.Submitted, UserId.ToString(), SystemTime.UtcNow);
                    t.UpdateStatus(NoteStatus.Approved, UserId.ToString(), SystemTime.UtcNow);
                }).WithTonnages(newTransferNoteTonnage2).Create();

                // create another transfer from note 1 but is rejected so wont be included
                var newTransferNoteTonnage3 = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(transferTonnage1.Id, 1002, null)
                };

                TransferEvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(t =>
                {
                    t.UpdateStatus(NoteStatus.Submitted, UserId.ToString(), SystemTime.UtcNow);
                    t.UpdateStatus(NoteStatus.Rejected, UserId.ToString(), SystemTime.UtcNow);
                }).WithTonnages(newTransferNoteTonnage3).Create();

                //This will use the last bit of tonnage
                var editingTransferNoteTonnage = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(transferTonnage1.Id, 1, null)
                };
                var editingTransferNote = TransferEvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .WithOrganisation(transferOrganisation.Id)
                    .WithTonnages(editingTransferNoteTonnage).Create();

                updatedRecipientOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(updatedRecipientOrganisation.Id).Create();

                // now to call the request to edit the tonnage, should be allowed as this is an edit
                transferTonnageValues = new List<TransferTonnageValue>()
                {
                    new TransferTonnageValue(transferTonnage1.Id, WeeeCategory.ConsumerEquipment.ToInt(), 1, null, Guid.Empty)
                };

                request = new EditTransferEvidenceNoteRequest(editingTransferNote.Id, editingTransferNote.OrganisationId, updatedRecipientOrganisation.Id, transferTonnageValues, EA.Weee.Core.AatfEvidence.NoteStatus.Draft);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                note = Query.GetTransferEvidenceNoteById(result);
            };

            private readonly It shouldHaveUpdatedTheTransferEvidenceNoteWithExpectedPropertyValues = () =>
            {
                ShouldMapToNote();
                note.Status.Should().Be(NoteStatus.Draft);
                note.NoteTransferTonnage.Count.Should().Be(1);
                foreach (var transferTonnageValue in transferTonnageValues)
                {
                    var tonnage = note.NoteTransferTonnage.FirstOrDefault(ntt =>
                        ntt.NoteTonnageId.Equals(transferTonnageValue.Id));

                    tonnage.Should().NotBeNull();
                    tonnage.Received.Should().Be(transferTonnageValue.FirstTonnage);
                    tonnage.Reused.Should().Be(transferTonnageValue.SecondTonnage);
                }
                note.Status.Should().Be(NoteStatus.Draft);
            };

            private readonly It shouldHaveNotSetApprovedDetails = () =>
            {
                note.ApprovedRecipientAddress.Should().BeNull();
                note.ApprovedRecipientSchemeName.Should().BeNull();
                note.ApprovedTransfererAddress.Should().BeNull();
                note.ApprovedTransfererSchemeName.Should().BeNull();
            };
        }

        [Component]
        public class WhenIEditADraftTransferEvidenceNoteWhereThereIsNoAvailableTonnage : EditTransferEvidenceNoteHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                transferOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(transferOrganisation.Id).Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, transferOrganisation.Id).Create();

                existingRecipientOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(existingRecipientOrganisation.Id).Create();

                var existingTonnagesNote1 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, 5, 1)
                };

                var note1 = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .WithTonnages(existingTonnagesNote1).Create();

                var transferTonnage1 =
                    note1.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ConsumerEquipment));

                // create transfer from note 1
                var newTransferNoteTonnage1 = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(transferTonnage1.Id, 2, null)
                };

                TransferEvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(t =>
                {
                    t.UpdateStatus(NoteStatus.Submitted, UserId.ToString(), SystemTime.UtcNow);
                    t.UpdateStatus(NoteStatus.Approved, UserId.ToString(), SystemTime.UtcNow);
                }).WithTonnages(newTransferNoteTonnage1).Create();

                // create another transfer from note 1
                var newTransferNoteTonnage2 = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(transferTonnage1.Id, 2, null)
                };

                TransferEvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(t =>
                {
                    t.UpdateStatus(NoteStatus.Submitted, UserId.ToString(), SystemTime.UtcNow);
                    t.UpdateStatus(NoteStatus.Approved, UserId.ToString(), SystemTime.UtcNow);
                }).WithTonnages(newTransferNoteTonnage2).Create();

                // create another transfer from note 1 but is rejected so wont be included
                var newTransferNoteTonnage3 = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(transferTonnage1.Id, 1002, null)
                };

                TransferEvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(t =>
                {
                    t.UpdateStatus(NoteStatus.Submitted, UserId.ToString(), SystemTime.UtcNow);
                    t.UpdateStatus(NoteStatus.Rejected, UserId.ToString(), SystemTime.UtcNow);
                }).WithTonnages(newTransferNoteTonnage3).Create();

                //This will use the last bit of tonnage
                var editingTransferNoteTonnage = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(transferTonnage1.Id, 1, null)
                };
                var editingTransferNote = TransferEvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .WithOrganisation(transferOrganisation.Id)
                    .WithTonnages(editingTransferNoteTonnage).Create();

                updatedRecipientOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(updatedRecipientOrganisation.Id).Create();

                // now to call the request to edit the tonnage, should not be allowed as requesting more than available
                transferTonnageValues = new List<TransferTonnageValue>()
                {
                    new TransferTonnageValue(transferTonnage1.Id, WeeeCategory.ConsumerEquipment.ToInt(), 2, null, Guid.Empty)
                };

                request = new EditTransferEvidenceNoteRequest(editingTransferNote.Id, editingTransferNote.OrganisationId, updatedRecipientOrganisation.Id, transferTonnageValues, EA.Weee.Core.AatfEvidence.NoteStatus.Draft);
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => handler.HandleAsync(request));
            };

            private readonly It shouldHaveCaughtInvalidOperationException = ShouldThrowException<InvalidOperationException>;
        }

        public class EditTransferEvidenceNoteHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<EditTransferEvidenceNoteRequest, Guid> handler;
            protected static EditTransferEvidenceNoteRequest request;
            protected static Organisation transferOrganisation;
            protected static Organisation existingRecipientOrganisation;
            protected static Organisation updatedRecipientOrganisation;
            protected static Guid result;
            protected static Note note;
            protected static List<TransferTonnageValue> transferTonnageValues;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                    .WithExternalUserAccess();

                handler = Container.Resolve<IRequestHandler<EditTransferEvidenceNoteRequest, Guid>>();

                transferTonnageValues = new List<TransferTonnageValue>();

                return setup;
            }

            protected static void ShouldMapToNote()
            {
                note.CreatedById.Should().Be(UserId.ToString());
                note.Aatf.Should().BeNull();
                note.EndDate.Date.Should().BeSameDateAs(SystemTime.UtcNow.Date);
                note.StartDate.Date.Should().BeSameDateAs(SystemTime.UtcNow.Date);
                note.WasteType.Should().Be(WasteType.HouseHold);
                note.Protocol.Should().BeNull();
                note.Recipient.Id.Should().Be(updatedRecipientOrganisation.Id);
                note.Reference.Should().BeGreaterThan(0);
                note.CreatedDate.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromSeconds(10));
                note.Organisation.Should().Be(transferOrganisation);
                note.NoteType.Should().Be(NoteType.TransferNote);
                note.NoteTonnage.Count.Should().Be(0);
            }
        }
    }
}
