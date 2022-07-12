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
    using EA.Prsd.Core;
    using EA.Weee.Domain.Scheme;
    using EA.Weee.Requests.Admin;
    using FluentAssertions;
    using NUnit.Specifications;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using NoteStatus = Core.AatfEvidence.NoteStatus;
    using NoteStatusDomain = Domain.Evidence.NoteStatus;

    public class GetEvidenceNoteTransfersForInternalUserRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIGetOneSubmittedTransfreEvidenceNote : GetEvidenceNoteTransfersForInternalUserRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, organisation.Id).Create();
                scheme = SchemeDbSetup.Init().WithOrganisation(organisation.Id).Create();

                var categories = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 2, 1),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null)
                };

                note = EvidenceNoteDbSetup
                        .Init()
                        .WithTonnages(categories)
                        .With(n =>
                        {
                            n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                        }).Create();

                request = new GetEvidenceNoteTransfersForInternalUserRequest(note.Id);
            };

            private readonly Because of = () =>
            {
                data = Task.Run(async () => await handler.HandleAsync(request)).Result;

                note = Query.GetEvidenceNoteById(note.Id);
            };

            private readonly It shouldHaveReturnedTheTransferEvidenceNote = () =>
            {
                data.Should().NotBeNull();
            };

            private readonly It shouldHaveReturnedTheTransferEvidenceNoteWithExpectedPropertyValues = () =>
            {
                ShouldMapToNote();
                data.Status.Should().Be(NoteStatus.Submitted);
            };
        }

        [Component]
        public class WhenIGetOneApprovedTransfreEvidenceNote : GetEvidenceNoteTransfersForInternalUserRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, organisation.Id).Create();
                scheme = SchemeDbSetup.Init().WithOrganisation(organisation.Id).Create();

                var categories = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 2, 1),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null)
                };

                note = EvidenceNoteDbSetup
                        .Init()
                        .WithTonnages(categories)
                        .With(n =>
                        {
                            n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                            n.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString(), SystemTime.UtcNow);
                        }).Create();

                request = new GetEvidenceNoteTransfersForInternalUserRequest(note.Id);
            };

            private readonly Because of = () =>
            {
                data = Task.Run(async () => await handler.HandleAsync(request)).Result;

                note = Query.GetEvidenceNoteById(note.Id);
            };

            private readonly It shouldHaveReturnedTheTransferEvidenceNote = () =>
            {
                data.Should().NotBeNull();
            };

            private readonly It shouldHaveReturnedTheTransferEvidenceNoteWithExpectedPropertyValues = () =>
            {
                ShouldMapToNote();
                data.Status.Should().Be(NoteStatus.Approved);
            };
        }

        [Component]
        public class WhenIGetOneRejectedTransfreEvidenceNote : GetEvidenceNoteTransfersForInternalUserRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, organisation.Id).Create();
                scheme = SchemeDbSetup.Init().WithOrganisation(organisation.Id).Create();

                var categories = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 2, 1),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null)
                };

                note = EvidenceNoteDbSetup
                        .Init()
                        .WithTonnages(categories)
                        .With(n =>
                        {
                            n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                            n.UpdateStatus(NoteStatusDomain.Rejected, UserId.ToString(), SystemTime.UtcNow);
                        }).Create();

                request = new GetEvidenceNoteTransfersForInternalUserRequest(note.Id);
            };

            private readonly Because of = () =>
            {
                data = Task.Run(async () => await handler.HandleAsync(request)).Result;

                note = Query.GetEvidenceNoteById(note.Id);
            };

            private readonly It shouldHaveReturnedTheTransferEvidenceNote = () =>
            {
                data.Should().NotBeNull();
            };

            private readonly It shouldHaveReturnedTheTransferEvidenceNoteWithExpectedPropertyValues = () =>
            {
                ShouldMapToNote();
                data.Status.Should().Be(NoteStatus.Rejected);
            };
        }

        [Component]
        public class WhenIGetOneReturnedTransfreEvidenceNote : GetEvidenceNoteTransfersForInternalUserRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, organisation.Id).Create();
                scheme = SchemeDbSetup.Init().WithOrganisation(organisation.Id).Create();

                var categories = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 2, 1),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null)
                };

                note = EvidenceNoteDbSetup
                        .Init()
                        .WithTonnages(categories)
                        .With(n =>
                        {
                            n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                            n.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString(), SystemTime.UtcNow.AddMilliseconds(2));
                            n.UpdateStatus(NoteStatusDomain.Returned, UserId.ToString(), SystemTime.UtcNow.AddMilliseconds(400));
                        }).Create();

                request = new GetEvidenceNoteTransfersForInternalUserRequest(note.Id);
            };

            private readonly Because of = () =>
            {
                data = Task.Run(async () => await handler.HandleAsync(request)).Result;

                note = Query.GetEvidenceNoteById(note.Id);
            };

            private readonly It shouldHaveReturnedTheTransferEvidenceNote = () =>
            {
                data.Should().NotBeNull();
            };

            private readonly It shouldHaveReturnedTheTransferEvidenceNoteWithExpectedPropertyValues = () =>
            {
                ShouldMapToNote();
                data.Status.Should().Be(NoteStatus.Returned);
            };
        }

        public class GetEvidenceNoteTransfersForInternalUserRequestHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<GetEvidenceNoteTransfersForInternalUserRequest, TransferEvidenceNoteData> handler;
            protected static Organisation organisation;
            protected static GetEvidenceNoteTransfersForInternalUserRequest request;
            protected static TransferEvidenceNoteData data;
            protected static Scheme scheme;
            protected static Note note;
            protected static Fixture fixture;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                    .WithDefaultSettings(resetDb: true)
                    .WithInternalUserAccess(false);

                var authority = Query.GetEaCompetentAuthority();
                var role = Query.GetInternalUserRole();

                if (!Query.CompetentAuthorityUserExists(UserId.ToString(), role.Id))
                {
                    CompetentAuthorityUserDbSetup.Init().WithUserIdAndAuthorityAndRole(UserId.ToString(), authority.Id, role.Id)
                        .Create();
                }

                fixture = new Fixture();
                handler = Container.Resolve<IRequestHandler<GetEvidenceNoteTransfersForInternalUserRequest, TransferEvidenceNoteData>>();

                return setup;
            }

            protected static void ShouldMapToNote()
            {
                data.Id.Should().Be(note.Id);
                data.Reference.Should().Be(note.Reference);
                data.Status.Should().Be((NoteStatus)note.Status.Value);
                data.ComplianceYear.Should().Be(note.ComplianceYear);
                data.WasteType.ToInt().Should().Be(note.WasteType.ToInt());
                ((int)data.Type).Should().Be(note.NoteType.Value);
                data.SubmittedDate.Should().Be(note.NoteStatusHistory
                    .Where(n => n.ToStatus.Equals(NoteStatus.Submitted))
                    .OrderByDescending(n => n.ChangedDate).FirstOrDefault()
                    ?.ChangedDate);
                data.ApprovedDate.Should().Be(note.NoteStatusHistory
                    .Where(n => n.ToStatus.Equals(NoteStatus.Approved))
                    .OrderByDescending(n => n.ChangedDate).FirstOrDefault()
                    ?.ChangedDate);
                data.ReturnedDate.Should().Be(note.NoteStatusHistory
                    .Where(n => n.ToStatus.Equals(NoteStatus.Returned))
                    .OrderByDescending(n => n.ChangedDate).FirstOrDefault()
                    ?.ChangedDate);
                data.RejectedDate.Should().Be(note.NoteStatusHistory
                    .Where(n => n.ToStatus.Equals(EA.Weee.Domain.Evidence.NoteStatus.Rejected))
                    .OrderByDescending(n => n.ChangedDate).FirstOrDefault()
                    ?.ChangedDate);
                data.RecipientSchemeData.Should().NotBeNull();
                data.RecipientSchemeData.Id.Should().Be(note.Recipient.Id);
                data.TransferredOrganisationData.Should().NotBeNull();
                data.TransferredOrganisationData.Id.Should().Be(note.Organisation.Id);
                data.TransferredSchemeData.Should().NotBeNull();
                data.TransferredSchemeData.Should().Be(note.Organisation.Id);
                data.RecipientOrganisationData.Should().NotBeNull();
                data.RecipientOrganisationData.Id.Should().Be(note.Recipient.OrganisationId);

                foreach (var noteTonnage in note.NoteTransferTonnage)
                {
                    data.TransferEvidenceNoteTonnageData.Should().Contain(n => n.EvidenceTonnageData.Received.Equals(noteTonnage.NoteTonnage.Reused) &&
                    n.EvidenceTonnageData.Received.Equals(noteTonnage.NoteTonnage.Reused) && 
                    n.EvidenceTonnageData.TransferredReceived.Equals(noteTonnage.Received) &&
                    n.EvidenceTonnageData.TransferredReused.Equals(noteTonnage.Reused));
                }
            }
        }
    }
}
