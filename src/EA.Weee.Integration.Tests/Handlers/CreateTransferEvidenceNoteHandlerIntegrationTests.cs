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

    public class CreateTransferEvidenceNoteRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenICreateADraftTransferEvidenceNote : CreateTransferEvidenceNoteHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                scheme = SchemeDbSetup.Init().Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, organisation.Id).Create();

                recipient = SchemeDbSetup.Init().Create();

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
                    new TransferTonnageValue(Guid.Empty, WeeeCategory.DisplayEquipment.ToInt(), 1, null, transferTonnage1.Id),
                    new TransferTonnageValue(Guid.Empty, WeeeCategory.LargeHouseholdAppliances.ToInt(), 3, 1, transferTonnage2.Id),
                    new TransferTonnageValue(Guid.Empty, WeeeCategory.MonitoringAndControlInstruments.ToInt(), 7, 1, transferTonnage3.Id),
                };

                request = new TransferEvidenceNoteRequest(organisation.Id, recipient.Id, new List<int>()
                    {
                        WeeeCategory.DisplayEquipment.ToInt(),
                        WeeeCategory.LargeHouseholdAppliances.ToInt(),
                        WeeeCategory.MonitoringAndControlInstruments.ToInt(),
                    }, 
                    transferTonnageValues,
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

            private readonly It shouldHaveCreatedNoteCategories = () =>
            {
                note.NoteTransferCategories.Count.Should().Be(3);
                note.NoteTransferCategories.First(n => n.CategoryId.Equals(WeeeCategory.DisplayEquipment)).Should()
                    .NotBeNull();
                note.NoteTransferCategories.First(n => n.CategoryId.Equals(WeeeCategory.LargeHouseholdAppliances)).Should()
                    .NotBeNull();
                note.NoteTransferCategories.First(n => n.CategoryId.Equals(WeeeCategory.MonitoringAndControlInstruments)).Should()
                    .NotBeNull();
            };
        }

        [Component]
        public class WhenICreateADraftTransferEvidenceNoteAgainstNotesWithTonnageTransfer : CreateTransferEvidenceNoteHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                scheme = SchemeDbSetup.Init().Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, organisation.Id).Create();

                recipient = SchemeDbSetup.Init().Create();

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
                    t.UpdateStatus(NoteStatus.Submitted, UserId.ToString());
                    t.UpdateStatus(NoteStatus.Approved, UserId.ToString());
                }).WithTonnages(newTransferNoteTonnage1).Create();

                // create another transfer from note 1
                var newTransferNoteTonnage2 = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(transferTonnage1.Id, 2, null)
                };

                TransferEvidenceNoteDbSetup.Init().With(t =>
                {
                    t.UpdateStatus(NoteStatus.Submitted, UserId.ToString());
                    t.UpdateStatus(NoteStatus.Approved, UserId.ToString());
                }).WithTonnages(newTransferNoteTonnage2).Create();

                // create another transfer from note 1 but not is not approved so wont be included
                var newTransferNoteTonnage3 = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(transferTonnage1.Id, 1002, null)
                };

                TransferEvidenceNoteDbSetup.Init().With(t =>
                {
                    t.UpdateStatus(NoteStatus.Submitted, UserId.ToString());
                }).WithTonnages(newTransferNoteTonnage3).Create();

                // now to call the request to take the last 1 tonne
                transferTonnageValues = new List<TransferTonnageValue>()
                {
                    new TransferTonnageValue(Guid.Empty, WeeeCategory.ConsumerEquipment.ToInt(), 1, null, transferTonnage1.Id)
                };

                request = new TransferEvidenceNoteRequest(organisation.Id, recipient.Id, new List<int>()
                    {
                        WeeeCategory.ConsumerEquipment.ToInt()
                    },
                    transferTonnageValues,
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

            private readonly It shouldHaveCreatedNoteCategories = () =>
            {
                note.NoteTransferCategories.Count.Should().Be(1);
                note.NoteTransferCategories.First(n => n.CategoryId.Equals(WeeeCategory.ConsumerEquipment)).Should()
                    .NotBeNull();
            };
        }

        [Component]
        [Ignore("Re-instate when available tonnages is returned to the creation screen")]
        //TODO: Re-instate when available tonnages is returned to the creation screen
        public class WhenICreateADraftTransferEvidenceNoteWhereThereIsNoAvailableTonnage : CreateTransferEvidenceNoteHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                scheme = SchemeDbSetup.Init().Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, organisation.Id).Create();

                recipient = SchemeDbSetup.Init().Create();

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
                    t.UpdateStatus(NoteStatus.Submitted, UserId.ToString());
                    t.UpdateStatus(NoteStatus.Approved, UserId.ToString());
                }).WithTonnages(newTransferNoteTonnage).Create();

                transferTonnageValues = new List<TransferTonnageValue>()
                {
                    new TransferTonnageValue(Guid.Empty, WeeeCategory.DisplayEquipment.ToInt(), 1, null, transferTonnage1.Id)
                };

                request = new TransferEvidenceNoteRequest(organisation.Id, recipient.Id, 
                    new List<int>()
                    {
                        WeeeCategory.DisplayEquipment.ToInt()
                    },
                    transferTonnageValues,
                    Core.AatfEvidence.NoteStatus.Draft);
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => handler.HandleAsync(request));
            };

            private readonly It shouldHaveCaughtSecurityException = ShouldThrowException<InvalidOperationException>;
        }

        public class CreateTransferEvidenceNoteHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<TransferEvidenceNoteRequest, Guid> handler;
            protected static Organisation organisation;
            protected static TransferEvidenceNoteRequest request;
            protected static Scheme scheme;
            protected static Scheme recipient;
            protected static Guid result;
            protected static Note note;
            protected static Fixture fixture;
            protected static List<TransferTonnageValue> transferTonnageValues;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                    .WithExternalUserAccess();

                fixture = new Fixture();
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
                note.WasteType.Should().BeNull();
                note.Protocol.Should().BeNull();
                note.Recipient.Should().Be(recipient);
                note.Reference.Should().BeGreaterThan(0);
                note.CreatedDate.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromSeconds(10));
                note.Organisation.Should().Be(organisation);
                note.NoteType.Should().Be(NoteType.TransferNote);
                note.NoteTonnage.Count.Should().Be(0);
                note.NoteTransferTonnage.Count.Should().Be(transferTonnageValues.Count);
                foreach (var transferTonnageValue in transferTonnageValues)
                {
                    var tonnage = note.NoteTransferTonnage.FirstOrDefault(ntt =>
                        ntt.NoteTonnageId.Equals(transferTonnageValue.TransferTonnageId));

                    tonnage.Should().NotBeNull();
                    tonnage.Received.Should().Be(transferTonnageValue.FirstTonnage);
                    tonnage.Reused.Should().Be(transferTonnageValue.SecondTonnage);
                }
            }
        }
    }
}
