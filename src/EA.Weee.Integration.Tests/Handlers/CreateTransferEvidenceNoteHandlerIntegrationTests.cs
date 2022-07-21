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
    using Core.Aatf;
    using Core.AatfEvidence;
    using Core.Helpers;
    using Domain.Evidence;
    using Domain.Lookup;
    using Domain.Organisation;
    using Domain.Scheme;
    using FluentAssertions;
    using NUnit.Framework;
    using NUnit.Specifications;
    using Prsd.Core;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using Requests.AatfEvidence;
    using Requests.Scheme;
    using NoteStatus = Domain.Evidence.NoteStatus;
    using NoteType = Domain.Evidence.NoteType;
    using WasteType = Domain.Evidence.WasteType;

    public class CreateTransferEvidenceNoteRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenICreateADraftTransferEvidenceNote : CreateTransferEvidenceNoteHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                transferOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(transferOrganisation.Id).Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, transferOrganisation.Id).Create();

                recipientOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation.Id).Create();

                var existingTonnagesNote1 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, 2, 1),
                    new NoteTonnage(WeeeCategory.DisplayEquipment, 3, 1),
                };

                var note1 = EvidenceNoteDbSetup.Init().WithTonnages(existingTonnagesNote1).Create();

                var existingTonnagesNote2 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 5, 1),
                    new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 10, 1),
                };

                var note2 = EvidenceNoteDbSetup.Init().WithTonnages(existingTonnagesNote2).Create();

                var transferTonnage1 =
                    note1.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.DisplayEquipment));

                var transferTonnage2 =
                    note2.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.LargeHouseholdAppliances));
                var transferTonnage3 =
                    note2.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.MonitoringAndControlInstruments));

                transferTonnageValues = new List<TransferTonnageValue>()
                {
                    new TransferTonnageValue(transferTonnage1.Id, WeeeCategory.DisplayEquipment.ToInt(), 1, null, Guid.Empty),
                    new TransferTonnageValue(transferTonnage2.Id, WeeeCategory.LargeHouseholdAppliances.ToInt(), 3, 1, Guid.Empty),
                    new TransferTonnageValue(transferTonnage3.Id, WeeeCategory.MonitoringAndControlInstruments.ToInt(), 7, 1, Guid.Empty),
                };

                request = new TransferEvidenceNoteRequest(transferOrganisation.Id, recipientOrganisation.Id, new List<int>()
                    {
                        WeeeCategory.DisplayEquipment.ToInt(),
                        WeeeCategory.LargeHouseholdAppliances.ToInt(),
                        WeeeCategory.MonitoringAndControlInstruments.ToInt(),
                    }, 
                    transferTonnageValues,
                    new List<Guid>() { note1.Id },
                EA.Weee.Core.AatfEvidence.NoteStatus.Draft);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                note = Query.GetEvidenceNoteById(result);
            };

            private readonly It shouldHaveCreatedTheTransferEvidenceNote = () =>
            {
                note.Should().NotBeNull();
            };

            private readonly It shouldHaveCreatedTheTransferEvidenceNoteWithExpectedPropertyValues = () =>
            {
                ShouldMapToNote();
                note.Status.Should().Be(NoteStatus.Draft);
            };
        }

        [Component]
        public class WhenICreateADraftTransferEvidenceNoteAgainstNotesWithTonnageTransfer : CreateTransferEvidenceNoteHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                transferOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(transferOrganisation.Id).Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, transferOrganisation.Id).Create();

                recipientOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation.Id).Create();

                var existingTonnagesNote1 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, 5, 1)
                };

                var note1 = EvidenceNoteDbSetup.Init().WithTonnages(existingTonnagesNote1).Create();
                var transferTonnage1 =
                    note1.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ConsumerEquipment));

                // create transfer from note 1
                var newTransferNoteTonnage1 = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(transferTonnage1.Id, 2, null)
                };

                TransferEvidenceNoteDbSetup.Init().With(t =>
                {
                    t.UpdateStatus(NoteStatus.Submitted, UserId.ToString(), SystemTime.UtcNow);
                    t.UpdateStatus(NoteStatus.Approved, UserId.ToString(), SystemTime.UtcNow);
                }).WithTonnages(newTransferNoteTonnage1).Create();

                // create another transfer from note 1
                var newTransferNoteTonnage2 = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(transferTonnage1.Id, 2, null)
                };

                TransferEvidenceNoteDbSetup.Init().With(t =>
                {
                    t.UpdateStatus(NoteStatus.Submitted, UserId.ToString(), SystemTime.UtcNow);
                    t.UpdateStatus(NoteStatus.Approved, UserId.ToString(), SystemTime.UtcNow);
                }).WithTonnages(newTransferNoteTonnage2).Create();

                // create another transfer from note 1 but is rejected so wont be included
                var newTransferNoteTonnage3 = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(transferTonnage1.Id, 1002, null)
                };

                TransferEvidenceNoteDbSetup.Init().With(t =>
                {
                    t.UpdateStatus(NoteStatus.Rejected, UserId.ToString(), SystemTime.UtcNow);
                }).WithTonnages(newTransferNoteTonnage3).Create();

                // now to call the request to take the last 1 tonne
                transferTonnageValues = new List<TransferTonnageValue>()
                {
                    new TransferTonnageValue(transferTonnage1.Id, WeeeCategory.ConsumerEquipment.ToInt(), 1, null, Guid.Empty)
                };

                request = new TransferEvidenceNoteRequest(transferOrganisation.Id, recipientOrganisation.Id, new List<int>()
                    {
                        WeeeCategory.ConsumerEquipment.ToInt()
                    },
                    transferTonnageValues,
                    new List<Guid>() { note1.Id },
                    EA.Weee.Core.AatfEvidence.NoteStatus.Draft);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                note = Query.GetEvidenceNoteById(result);
            };

            private readonly It shouldHaveCreatedTheTransferEvidenceNote = () =>
            {
                note.Should().NotBeNull();
            };

            private readonly It shouldHaveCreatedTheTransferEvidenceNoteWithExpectedPropertyValues = () =>
            {
                ShouldMapToNote();
                note.Status.Should().Be(NoteStatus.Draft);
            };
        }

        [Component]
        public class WhenICreateADraftTransferEvidenceNoteWhereThereIsNoAvailableTonnage : CreateTransferEvidenceNoteHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                transferOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(transferOrganisation.Id).Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, transferOrganisation.Id).Create();

                recipientOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation.Id).Create();

                var existingTonnagesNote1 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, 2, 1),
                };

                var note1 = EvidenceNoteDbSetup.Init().WithTonnages(existingTonnagesNote1).Create();
                var transferTonnage1 =
                    note1.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ConsumerEquipment));

                // someone has come along in and created a new transfer note with same tonnage, hence reducing the amount available below that being requested
                var newTransferNoteTonnage = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(transferTonnage1.Id, 2, null)
                };

                TransferEvidenceNoteDbSetup.Init().With(t =>
                {
                    t.UpdateStatus(NoteStatus.Submitted, UserId.ToString(), SystemTime.UtcNow);
                    t.UpdateStatus(NoteStatus.Approved, UserId.ToString(), SystemTime.UtcNow);
                }).WithTonnages(newTransferNoteTonnage).Create();

                transferTonnageValues = new List<TransferTonnageValue>()
                {
                    new TransferTonnageValue(transferTonnage1.Id, WeeeCategory.DisplayEquipment.ToInt(), 1, null, Guid.Empty)
                };

                request = new TransferEvidenceNoteRequest(transferOrganisation.Id, recipientOrganisation.Id, 
                    new List<int>()
                    {
                        WeeeCategory.DisplayEquipment.ToInt()
                    },
                    transferTonnageValues,
                    new List<Guid>() { note1.Id },
                    Core.AatfEvidence.NoteStatus.Draft);
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => handler.HandleAsync(request));
            };

            private readonly It shouldHaveCaughtInvalidOperationException = ShouldThrowException<InvalidOperationException>;
        }

        public class CreateTransferEvidenceNoteHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<TransferEvidenceNoteRequest, Guid> handler;
            protected static TransferEvidenceNoteRequest request;
            protected static Organisation transferOrganisation;
            protected static Organisation recipientOrganisation;
            protected static Guid result;
            protected static Note note;
            protected static List<TransferTonnageValue> transferTonnageValues;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                    .WithExternalUserAccess();

                handler = Container.Resolve<IRequestHandler<TransferEvidenceNoteRequest, Guid>>();

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
                note.Recipient.Should().Be(recipientOrganisation);
                note.Reference.Should().BeGreaterThan(0);
                note.CreatedDate.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromSeconds(10));
                note.Organisation.Should().Be(transferOrganisation);
                note.NoteType.Should().Be(NoteType.TransferNote);
                note.NoteTonnage.Count.Should().Be(0);
                note.NoteTransferTonnage.Count.Should().Be(transferTonnageValues.Count);
                foreach (var transferTonnageValue in transferTonnageValues)
                {
                    var tonnage = note.NoteTransferTonnage.FirstOrDefault(ntt =>
                        ntt.NoteTonnageId.Equals(transferTonnageValue.Id));

                    tonnage.Should().NotBeNull();
                    tonnage.Received.Should().Be(transferTonnageValue.FirstTonnage);
                    tonnage.Reused.Should().Be(transferTonnageValue.SecondTonnage);
                }
            }
        }
    }
}
