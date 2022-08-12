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
    using Prsd.Core;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using Requests.AatfEvidence;
    using NoteStatus = Domain.Evidence.NoteStatus;
    using NoteType = Core.AatfEvidence.NoteType;

    public class GetEvidenceNoteForSchemeHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIGetADraftEvidenceNoteAsScheme : GetEvidenceNoteForSchemeHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                recipientOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation.Id).Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, recipientOrganisation.Id).Create();

                var categories = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 2, 1),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 0, 0)
                };

                note = EvidenceNoteDbSetup
                    .Init().WithTonnages(categories)
                    .WithRecipient(recipientOrganisation.Id)
                    .WithOrganisation(organisation.Id).Create();

                request = new GetEvidenceNoteForSchemeRequest(note.Id);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                note = Query.GetEvidenceNoteById(note.Id);
            };

            private readonly It shouldHaveReturnedTheEvidenceNote = () =>
            {
                result.Should().NotBeNull();
            };

            private readonly It shouldHaveReturnedTheEvidenceNoteWithExpectedPropertyValues = () =>
            {
                ShouldMapToNote();
                result.Status.Should().Be(EA.Weee.Core.AatfEvidence.NoteStatus.Draft);
            };
        }

        [Component]
        public class WhenIGetASubmittedEvidenceNoteAsScheme : GetEvidenceNoteForSchemeHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(organisation.Id).Create();

                recipientOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation.Id).Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, recipientOrganisation.Id).Create();

                var categories = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 2, 1),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 0, 0)
                };

                note = EvidenceNoteDbSetup.Init().WithTonnages(categories)
                    .WithOrganisation(organisation.Id)
                    .WithRecipient(recipientOrganisation.Id)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatus.Submitted, UserId.ToString(), SystemTime.UtcNow);
                    })
                    .Create();
                
                request = new GetEvidenceNoteForSchemeRequest(note.Id);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                note = Query.GetEvidenceNoteById(note.Id);
            };

            private readonly It shouldHaveCreatedEvidenceNote = () =>
            {
                result.Should().NotBeNull();
            };

            private readonly It shouldHaveCreatedTheEvidenceNoteWithExpectedPropertyValues = () =>
            {
                ShouldMapToNote();
                result.Status.Should().Be(EA.Weee.Core.AatfEvidence.NoteStatus.Submitted);
                result.SubmittedDate.Value.ToShortDateString().Should().Be(note.NoteStatusHistory.First(n => n.ToStatus.Equals(NoteStatus.Submitted)).ChangedDate.ToShortDateString());
            };
        }

        [Component]
        public class WhenIGetOneReturnedEvidenceNoteAsScheme : GetEvidenceNoteForSchemeHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                scheme = SchemeDbSetup.Init().WithOrganisation(organisation.Id).Create();

                recipientOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation.Id).Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, recipientOrganisation.Id).Create();

                var categories = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 2, 1),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 0, 0)
                };

                note = EvidenceNoteDbSetup.Init().WithTonnages(categories)
                    .WithOrganisation(organisation.Id)
                    .WithRecipient(recipientOrganisation.Id)
                     .With(n =>
                     {
                         n.UpdateStatus(NoteStatus.Returned, UserId.ToString(), SystemTime.UtcNow, "reason returned");
                     })
                    .Create();

                request = new GetEvidenceNoteForSchemeRequest(note.Id);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                note = Query.GetEvidenceNoteById(note.Id);
            };

            private readonly It shouldHaveCreatedEvidenceNote = () =>
            {
                result.Should().NotBeNull();
            };

            private readonly It shouldHaveCreatedTheEvidenceNoteWithExpectedPropertyValues = () =>
            {
                ShouldMapToNote();
                result.Status.Should().Be(EA.Weee.Core.AatfEvidence.NoteStatus.Returned);
                result.ReturnedReason.Should().Be("reason returned");
                result.ReturnedDate.Value.ToShortDateString().Should().Be(note.NoteStatusHistory.First(n => n.ToStatus.Equals(NoteStatus.Returned)).ChangedDate.ToShortDateString());
            };
        }

        [Component]
        public class WhenIGetOneRejectedEvidenceNoteAsScheme : GetEvidenceNoteForSchemeHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                scheme = SchemeDbSetup.Init().WithOrganisation(organisation.Id).Create();

                recipientOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation.Id).Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, recipientOrganisation.Id).Create();

                var categories = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 2, 1),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 0, 0)
                };

                note = EvidenceNoteDbSetup.Init().WithTonnages(categories)
                    .WithOrganisation(organisation.Id)
                    .WithRecipient(recipientOrganisation.Id)
                     .With(n =>
                     {
                         n.UpdateStatus(NoteStatus.Rejected, UserId.ToString(), SystemTime.UtcNow, "reason rejected");
                     })
                    .Create();

                request = new GetEvidenceNoteForSchemeRequest(note.Id);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                note = Query.GetEvidenceNoteById(note.Id);
            };

            private readonly It shouldHaveCreatedEvidenceNote = () =>
            {
                result.Should().NotBeNull();
            };

            private readonly It shouldHaveCreatedTheEvidenceNoteWithExpectedPropertyValues = () =>
            {
                ShouldMapToNote();
                result.Status.Should().Be(EA.Weee.Core.AatfEvidence.NoteStatus.Rejected);
                result.RejectedReason.Should().Be("reason rejected");
                result.RejectedDate.Value.ToShortDateString().Should().Be(note.NoteStatusHistory.First(n => n.ToStatus.Equals(NoteStatus.Rejected)).ChangedDate.ToShortDateString());
            };
        }

        [Component]
        public class WhenIGetOneApprovedEvidenceNoteAsSchemeThatHasTransferHistory : GetEvidenceNoteForSchemeHandlerIntegrationTestBase
        {
            private static Note transferNote1;
            private static Note transferNote2;
            private static DateTime currentDate;

            private readonly Establish context = () =>
            {
                LocalSetup();

                currentDate = SystemTime.UtcNow;
                organisation = OrganisationDbSetup.Init().Create();
                scheme = SchemeDbSetup.Init().WithOrganisation(organisation.Id).Create();

                recipientOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation.Id).Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, recipientOrganisation.Id).Create();

                var categories = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 50, 1),
                    new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 100, 1),
                    new NoteTonnage(WeeeCategory.DisplayEquipment, 100, 1),
                };

                note = EvidenceNoteDbSetup.Init().WithTonnages(categories)
                    .WithOrganisation(organisation.Id)
                    .WithRecipient(recipientOrganisation.Id)
                     .With(n =>
                     {
                         n.UpdateStatus(NoteStatus.Submitted, UserId.ToString(), currentDate);
                         n.UpdateStatus(NoteStatus.Approved, UserId.ToString(), currentDate);
                     })
                    .Create();

                var transferTonnage1 =
                    note.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.AutomaticDispensers));

                // create transfer from note 1
                var newTransferNoteTonnage1 = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(transferTonnage1.Id, 10, null)
                };

                transferNote1 = TransferEvidenceNoteDbSetup.Init().With(t =>
                {
                    t.UpdateStatus(NoteStatus.Submitted, UserId.ToString(), currentDate);
                    t.UpdateStatus(NoteStatus.Rejected, UserId.ToString(), currentDate);
                }).WithTonnages(newTransferNoteTonnage1).Create();

                var transferTonnage2 =
                    note.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.CoolingApplicancesContainingRefrigerants));
                // create transfer from note 1
                var newTransferNoteTonnage2 = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(transferTonnage2.Id, 20, null)
                };

                transferNote2 = TransferEvidenceNoteDbSetup.Init().With(t =>
                {
                    t.UpdateStatus(NoteStatus.Submitted, UserId.ToString(), currentDate);
                }).WithTonnages(newTransferNoteTonnage2).Create();

                request = new GetEvidenceNoteForSchemeRequest(note.Id);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                note = Query.GetEvidenceNoteById(note.Id);
            };

            private readonly It shouldHaveCreatedEvidenceNote = () =>
            {
                result.Should().NotBeNull();
            };

            private readonly It shouldHaveCreatedTheEvidenceNoteWithExpectedPropertyValues = ShouldMapToNote;

            private readonly It shouldHaveCreatedTheEvidenceNoteTransferHistory = () =>
            {
                result.EvidenceNoteHistoryData.Count.Should().Be(2);

                result.EvidenceNoteHistoryData.Should()
                    .Contain(e => e.Id == transferNote1.Id
                                  && e.Status == transferNote1.Status
                                      .ToCoreEnumeration<EA.Weee.Core.AatfEvidence.NoteStatus>()
                                  && e.Type == NoteType.Transfer
                                  && e.Reference == transferNote1.Reference &&
                                  e.SubmittedDate.GetValueOrDefault().Date == currentDate.Date);

                result.EvidenceNoteHistoryData.Should()
                    .Contain(e => e.Id == transferNote2.Id
                                  && e.Status == transferNote2.Status
                                      .ToCoreEnumeration<EA.Weee.Core.AatfEvidence.NoteStatus>()
                                  && e.Type == NoteType.Transfer
                                  && e.Reference == transferNote2.Reference &&
                                  e.SubmittedDate.GetValueOrDefault().Date == currentDate.Date);
            };
        }

        [Component]
        public class WhenIGetOneVoidedEvidenceNote : GetEvidenceNoteForSchemeHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                scheme = SchemeDbSetup.Init().WithOrganisation(organisation.Id).Create();

                recipientOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation.Id).Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, recipientOrganisation.Id).Create();

                var categories = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 2, 1),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 0, 0)
                };

                note = EvidenceNoteDbSetup.Init().WithTonnages(categories)
                    .WithOrganisation(organisation.Id)
                    .WithRecipient(recipientOrganisation.Id)
                     .With(n =>
                     {
                         n.UpdateStatus(NoteStatus.Void, UserId.ToString(), SystemTime.UtcNow, "reason voided");
                     })
                    .Create();

                request = new GetEvidenceNoteForSchemeRequest(note.Id);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                note = Query.GetEvidenceNoteById(note.Id);
            };

            private readonly It shouldHaveCreatedVoidedEvidenceNote = () =>
            {
                result.Should().NotBeNull();
            };

            private readonly It shouldHaveCreatedTheVoidedEvidenceNoteWithExpectedPropertyValues = () =>
            {
                ShouldMapToNote();
                result.Status.Should().Be(EA.Weee.Core.AatfEvidence.NoteStatus.Void);
                result.VoidedReason.Should().Be(note.NoteStatusHistory
                    .Where(n => n.ToStatus.Equals(NoteStatus.Void))
                    .OrderByDescending(n => n.ChangedDate).FirstOrDefault()?.Reason);
                result.VoidedDate.Value.Date.Should().Be(note.NoteStatusHistory
                    .Where(n => n.ToStatus.Equals(NoteStatus.Void))
                    .OrderByDescending(n => n.ChangedDate).FirstOrDefault()?.ChangedDate.Date);
            };
        }

        [Component]
        public class WhenIGetOneNoteWhereNoteDoesNotExist : GetEvidenceNoteForSchemeHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                request = new GetEvidenceNoteForSchemeRequest(Guid.NewGuid());
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => handler.HandleAsync(request));
            };

            private readonly It shouldHaveCaughtArgumentNullException = ShouldThrowException<ArgumentNullException>;
        }

        [Component]
        public class WhenIGetOneNotesWhereUserIsNotAuthorised : GetEvidenceNoteForSchemeHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                note = EvidenceNoteDbSetup.Init().Create();

                request = new GetEvidenceNoteForSchemeRequest(note.Id);
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => handler.HandleAsync(request));
            };

            private readonly It shouldHaveCaughtArgumentException = ShouldThrowException<SecurityException>;
        }

        public class GetEvidenceNoteForSchemeHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<GetEvidenceNoteForSchemeRequest, EvidenceNoteData> handler;
            protected static Organisation organisation;
            protected static Organisation recipientOrganisation;
            protected static GetEvidenceNoteForSchemeRequest request;
            protected static EvidenceNoteData result;
            protected static Scheme scheme;
            protected static Note note;
            protected static Fixture fixture;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                    .WithExternalUserAccess();

                fixture = new Fixture();
                handler = Container.Resolve<IRequestHandler<GetEvidenceNoteForSchemeRequest, EvidenceNoteData>>();

                return setup;
            }
            protected static void ShouldMapToNote()
            {
                result.EndDate.Should().Be(note.EndDate);
                result.StartDate.Should().Be(note.StartDate);
                result.Reference.Should().Be(note.Reference);
                result.Protocol.ToInt().Should().Be(note.Protocol.ToInt());
                result.WasteType.ToInt().Should().Be(note.WasteType.ToInt());
                result.AatfData.Should().NotBeNull();
                result.AatfData.Id.Should().Be(note.Aatf.Id);
                result.RecipientSchemeData.Should().NotBeNull();
                var recipientScheme = Query.GetSchemeByOrganisationId(recipientOrganisation.Id);
                result.RecipientSchemeData.Id.Should().Be(recipientScheme.Id);
                result.EvidenceTonnageData.Count.Should().Be(3);
                result.OrganisationData.Should().NotBeNull();
                result.OrganisationData.Id.Should().Be(note.Organisation.Id);
                ((int)result.Type).Should().Be(note.NoteType.Value);
                result.Id.Should().Be(note.Id);
                result.ComplianceYear.Should().Be(note.ComplianceYear);
                foreach (var noteTonnage in note.NoteTonnage)
                {
                    result.EvidenceTonnageData.Should().Contain(n => n.Received.Equals(noteTonnage.Received) &&
                                                             n.Reused.Equals(noteTonnage.Reused) &&
                                                             ((int)n.CategoryId).Equals((int)noteTonnage.CategoryId));
                }
                result.RecipientOrganisationData.Should().NotBeNull();
                result.RecipientOrganisationData.Id.Should().Be(note.Recipient.Id);
            }
        }
    }
}
