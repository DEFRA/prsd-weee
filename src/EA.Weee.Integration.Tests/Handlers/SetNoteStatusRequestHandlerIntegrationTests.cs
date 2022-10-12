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
    using Domain.Evidence;
    using Domain.Organisation;
    using EA.Weee.Domain.Lookup;
    using FluentAssertions;
    using NUnit.Specifications;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using Requests.AatfEvidence;
    using NoteStatus = Domain.Evidence.NoteStatus;

    public class SetNoteStatusRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIUpdateAnEvidenceNoteStatusToApproved : SetNoteStatusRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(organisation.Id).Create();

                var recipientOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation.Id).Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, recipientOrganisation.Id).Create();

                note = EvidenceNoteDbSetup.Init()
                    .WithRecipient(recipientOrganisation.Id)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatus.Submitted, UserId.ToString(), DateTime.UtcNow);
                    })
                    .Create();

                request = new SetNoteStatusRequest(note.Id, Core.AatfEvidence.NoteStatus.Approved);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                note = Query.GetEvidenceNoteById(note.Id);
            };

            private readonly It shouldHaveUpdatedTheEvidenceNoteWithStatusApproved = () =>
            {
                note.Status.Should().Be(NoteStatus.Approved);
            };

            private readonly It shouldHaveCreatedANoteStatusHistory = () =>
            {
                var history = Query.GetLatestNoteStatusHistoryForNote(note.Id);
                history.ChangedById.Should().Be(UserId.ToString());
                history.ChangedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
                history.FromStatus.Should().Be(NoteStatus.Submitted);
                history.ToStatus.Should().Be(NoteStatus.Approved);
            };

            private readonly It shouldHaveSetApprovedDetails = () =>
            {
                note.ApprovedRecipientSchemeName.Should().Be(note.Recipient.Scheme.SchemeName);
                note.ApprovedRecipientAddress.Should().Be($"{note.Recipient.Scheme.SchemeName}<br/>{note.Recipient.OrganisationName}<br/>{note.Recipient.BusinessAddress.Address1}<br/>{note.Recipient.BusinessAddress.Address2}<br/>{note.Recipient.BusinessAddress.TownOrCity}<br/>{note.Recipient.BusinessAddress.CountyOrRegion}<br/>{note.Recipient.BusinessAddress.Postcode}");
                
                note.ApprovedTransfererAddress.Should().BeNull();    
                note.ApprovedTransfererSchemeName.Should().BeNull();
            };
        }

        [Component]
        public class WhenIUpdateATransferNoteStatusToApproved : SetNoteStatusRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(organisation.Id).Create();

                var recipientOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation.Id).Create();
                
                var transfererOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(transfererOrganisation.Id).Create();

                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, recipientOrganisation.Id).Create();

                note = TransferEvidenceNoteDbSetup.Init()
                    .WithRecipient(recipientOrganisation.Id)
                    .WithOrganisation(transfererOrganisation.Id)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatus.Submitted, UserId.ToString(), DateTime.UtcNow);
                    })
                    .Create();

                request = new SetNoteStatusRequest(note.Id, Core.AatfEvidence.NoteStatus.Approved);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                note = Query.GetTransferEvidenceNoteById(note.Id);
            };

            private readonly It shouldHaveUpdatedTheEvidenceNoteWithStatusApproved = () =>
            {
                note.Status.Should().Be(NoteStatus.Approved);
            };

            private readonly It shouldHaveCreatedANoteStatusHistory = () =>
            {
                var history = Query.GetLatestNoteStatusHistoryForNote(note.Id);
                history.ChangedById.Should().Be(UserId.ToString());
                history.ChangedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
                history.FromStatus.Should().Be(NoteStatus.Submitted);
                history.ToStatus.Should().Be(NoteStatus.Approved);
            };

            private readonly It shouldHaveSetApprovedDetails = () =>
            {
                note.ApprovedRecipientSchemeName.Should().Be(note.Recipient.Scheme.SchemeName);
                note.ApprovedRecipientAddress.Should().Be($"{note.Recipient.Scheme.SchemeName}<br/>{note.Recipient.OrganisationName}<br/>{note.Recipient.BusinessAddress.Address1}<br/>{note.Recipient.BusinessAddress.Address2}<br/>{note.Recipient.BusinessAddress.TownOrCity}<br/>{note.Recipient.BusinessAddress.CountyOrRegion}<br/>{note.Recipient.BusinessAddress.Postcode}");

                note.ApprovedTransfererSchemeName.Should().Be(note.Organisation.Scheme.SchemeName);
                note.ApprovedTransfererAddress.Should().Be($"{note.Organisation.Scheme.SchemeName}<br/>{note.Organisation.OrganisationName}<br/>{note.Organisation.BusinessAddress.Address1}<br/>{note.Organisation.BusinessAddress.Address2}<br/>{note.Organisation.BusinessAddress.TownOrCity}<br/>{note.Organisation.BusinessAddress.CountyOrRegion}<br/>{note.Organisation.BusinessAddress.Postcode}");
            };
        }

        [Component]
        public class WhenIUpdateAnEvidenceNoteStatusToReturned : SetNoteStatusRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(organisation.Id).Create();

                var recipientOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation.Id).Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, recipientOrganisation.Id).Create();

                note = EvidenceNoteDbSetup.Init()
                    .WithRecipient(recipientOrganisation.Id)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatus.Submitted, UserId.ToString(), DateTime.UtcNow);
                    })
                    .Create();

                request = new SetNoteStatusRequest(note.Id, Core.AatfEvidence.NoteStatus.Returned, "reason returned");
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                note = Query.GetEvidenceNoteById(note.Id);
            };

            private readonly It shouldHaveUpdatedTheEvidenceNoteWithStatusReturned = () =>
            {
                note.Status.Should().Be(NoteStatus.Returned);
            };

            private readonly It shouldHaveCreatedANoteStatusHistory = () =>
            {
                var history = Query.GetLatestNoteStatusHistoryForNote(note.Id);
                history.ChangedById.Should().Be(UserId.ToString());
                history.ChangedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
                history.FromStatus.Should().Be(NoteStatus.Submitted);
                history.ToStatus.Should().Be(NoteStatus.Returned);
                history.Reason.Should().Be("reason returned");
            };

            private readonly It shouldHaveNotHaveSetApprovedDetails = () =>
            {
                note.ApprovedRecipientSchemeName.Should().BeNull();
                note.ApprovedRecipientAddress.Should().BeNull();
                note.ApprovedTransfererAddress.Should().BeNull();
                note.ApprovedTransfererSchemeName.Should().BeNull();
            };
        }

        [Component]
        public class WhenIUpdateATransferNoteStatusToReturned : SetNoteStatusRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(organisation.Id).Create();

                var transfererOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(transfererOrganisation.Id).Create();

                var recipientOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation.Id).Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, recipientOrganisation.Id).Create();

                note = TransferEvidenceNoteDbSetup.Init()
                    .WithRecipient(recipientOrganisation.Id)
                    .WithOrganisation(transfererOrganisation.Id)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatus.Submitted, UserId.ToString(), DateTime.UtcNow);
                    })
                    .Create();

                request = new SetNoteStatusRequest(note.Id, Core.AatfEvidence.NoteStatus.Returned, "reason returned");
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                note = Query.GetTransferEvidenceNoteById(note.Id);
            };

            private readonly It shouldHaveUpdatedTheEvidenceNoteWithStatusReturned = () =>
            {
                note.Status.Should().Be(NoteStatus.Returned);
            };

            private readonly It shouldHaveCreatedANoteStatusHistory = () =>
            {
                var history = Query.GetLatestNoteStatusHistoryForNote(note.Id);
                history.ChangedById.Should().Be(UserId.ToString());
                history.ChangedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
                history.FromStatus.Should().Be(NoteStatus.Submitted);
                history.ToStatus.Should().Be(NoteStatus.Returned);
                history.Reason.Should().Be("reason returned");
            };

            private readonly It shouldHaveNotHaveSetApprovedDetails = () =>
            {
                note.ApprovedRecipientSchemeName.Should().BeNull();
                note.ApprovedRecipientAddress.Should().BeNull();
                note.ApprovedTransfererAddress.Should().BeNull();
                note.ApprovedTransfererSchemeName.Should().BeNull();
            };
        }

        [Component]
        public class WhenIUpdateAnEvidenceNoteStatusToRejected : SetNoteStatusRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(organisation.Id).Create();

                var recipientOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation.Id).Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, recipientOrganisation.Id).Create();

                note = EvidenceNoteDbSetup.Init()
                    .WithRecipient(recipientOrganisation.Id)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatus.Submitted, UserId.ToString(), DateTime.UtcNow);
                    })
                    .Create();

                request = new SetNoteStatusRequest(note.Id, Core.AatfEvidence.NoteStatus.Rejected, "reason returned");
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                note = Query.GetEvidenceNoteById(note.Id);
            };

            private readonly It shouldHaveUpdatedTheEvidenceNoteWithStatusReturned = () =>
            {
                note.Status.Should().Be(NoteStatus.Rejected);
            };

            private readonly It shouldHaveCreatedANoteStatusHistory = () =>
            {
                var history = Query.GetLatestNoteStatusHistoryForNote(note.Id);
                history.ChangedById.Should().Be(UserId.ToString());
                history.ChangedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
                history.FromStatus.Should().Be(NoteStatus.Submitted);
                history.ToStatus.Should().Be(NoteStatus.Rejected);
                history.Reason.Should().Be("reason returned");
            };

            private readonly It shouldHaveNotHaveSetApprovedDetails = () =>
            {
                note.ApprovedRecipientSchemeName.Should().BeNull();
                note.ApprovedRecipientAddress.Should().BeNull();
                note.ApprovedTransfererAddress.Should().BeNull();
                note.ApprovedTransfererSchemeName.Should().BeNull();
            };
        }

        [Component]
        public class WhenIUpdateATransferNoteStatusToRejected : SetNoteStatusRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(organisation.Id).Create();

                var transfererOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(transfererOrganisation.Id).Create();

                var recipientOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation.Id).Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, recipientOrganisation.Id).Create();

                note = TransferEvidenceNoteDbSetup.Init()
                    .WithRecipient(recipientOrganisation.Id)
                    .WithOrganisation(transfererOrganisation.Id)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatus.Submitted, UserId.ToString(), DateTime.UtcNow);
                    })
                    .Create();

                request = new SetNoteStatusRequest(note.Id, Core.AatfEvidence.NoteStatus.Rejected, "reason returned");
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                note = Query.GetTransferEvidenceNoteById(note.Id);
            };

            private readonly It shouldHaveUpdatedTheEvidenceNoteWithStatusReturned = () =>
            {
                note.Status.Should().Be(NoteStatus.Rejected);
            };

            private readonly It shouldHaveCreatedANoteStatusHistory = () =>
            {
                var history = Query.GetLatestNoteStatusHistoryForNote(note.Id);
                history.ChangedById.Should().Be(UserId.ToString());
                history.ChangedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
                history.FromStatus.Should().Be(NoteStatus.Submitted);
                history.ToStatus.Should().Be(NoteStatus.Rejected);
                history.Reason.Should().Be("reason returned");
            };

            private readonly It shouldHaveNotHaveSetApprovedDetails = () =>
            {
                note.ApprovedRecipientSchemeName.Should().BeNull();
                note.ApprovedRecipientAddress.Should().BeNull();
                note.ApprovedTransfererAddress.Should().BeNull();
                note.ApprovedTransfererSchemeName.Should().BeNull();
            };
        }

        [Component]
        public class WhenIUpdateAnEvidenceNoteStatusToSubmitted : SetNoteStatusRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(organisation.Id).Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, organisation.Id).Create();

                var recipientOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation.Id).Create();
                
                note = EvidenceNoteDbSetup.Init()
                    .WithRecipient(recipientOrganisation.Id)
                    .WithOrganisation(organisation.Id)
                    .Create();

                request = new SetNoteStatusRequest(note.Id, Core.AatfEvidence.NoteStatus.Submitted, null);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                note = Query.GetEvidenceNoteById(note.Id);
            };

            private readonly It shouldHaveUpdatedTheEvidenceNoteWithStatusReturned = () =>
            {
                note.Status.Should().Be(NoteStatus.Submitted);
            };

            private readonly It shouldHaveCreatedANoteStatusHistory = () =>
            {
                var history = Query.GetLatestNoteStatusHistoryForNote(note.Id);
                history.ChangedById.Should().Be(UserId.ToString());
                history.ChangedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
                history.FromStatus.Should().Be(NoteStatus.Draft);
                history.ToStatus.Should().Be(NoteStatus.Submitted);
                history.Reason.Should().BeNull();
            };

            private readonly It shouldHaveNotHaveSetApprovedDetails = () =>
            {
                note.ApprovedRecipientSchemeName.Should().BeNull();
                note.ApprovedRecipientAddress.Should().BeNull();
                note.ApprovedTransfererAddress.Should().BeNull();
                note.ApprovedTransfererSchemeName.Should().BeNull();
            };
        }

        [Component]
        public class WhenIUpdateATransferNoteStatusToSubmitted : SetNoteStatusRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(organisation.Id).Create();
                
                var transfererOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(transfererOrganisation.Id).Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, transfererOrganisation.Id).Create();
                
                var recipientOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation.Id).Create();
                
                note = TransferEvidenceNoteDbSetup.Init()
                    .WithRecipient(recipientOrganisation.Id)
                    .WithOrganisation(transfererOrganisation.Id)
                    .Create();

                request = new SetNoteStatusRequest(note.Id, Core.AatfEvidence.NoteStatus.Submitted, null);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                note = Query.GetTransferEvidenceNoteById(note.Id);
            };

            private readonly It shouldHaveUpdatedTheEvidenceNoteWithStatusReturned = () =>
            {
                note.Status.Should().Be(NoteStatus.Submitted);
            };

            private readonly It shouldHaveCreatedANoteStatusHistory = () =>
            {
                var history = Query.GetLatestNoteStatusHistoryForNote(note.Id);
                history.ChangedById.Should().Be(UserId.ToString());
                history.ChangedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
                history.FromStatus.Should().Be(NoteStatus.Draft);
                history.ToStatus.Should().Be(NoteStatus.Submitted);
                history.Reason.Should().BeNull();
            };

            private readonly It shouldHaveNotHaveSetApprovedDetails = () =>
            {
                note.ApprovedRecipientSchemeName.Should().BeNull();
                note.ApprovedRecipientAddress.Should().BeNull();
                note.ApprovedTransfererAddress.Should().BeNull();
                note.ApprovedTransfererSchemeName.Should().BeNull();
            };
        }

        [Component]
        public class WhenIUpdateATransferNoteWithZeroTonnagesStatusToSubmitted : SetNoteStatusRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(organisation.Id).Create();

                var transfererOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(transfererOrganisation.Id).Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, transfererOrganisation.Id).Create();

                var recipientOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation.Id).Create();

                var existingTonnagesNote1 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, 2, 1),
                };

                var note1 = EvidenceNoteDbSetup.Init().WithTonnages(existingTonnagesNote1).Create();
                var transferTonnage1 =
                    note1.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ConsumerEquipment));

                var newTransferNoteTonnage = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(transferTonnage1.Id, 0, 0)
                };

                note = TransferEvidenceNoteDbSetup.Init()
                    .WithRecipient(recipientOrganisation.Id)
                    .WithOrganisation(transfererOrganisation.Id)
                    .WithTonnages(newTransferNoteTonnage)
                    .Create();

                request = new SetNoteStatusRequest(note.Id, Core.AatfEvidence.NoteStatus.Submitted, null);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                note = Query.GetTransferEvidenceNoteById(note.Id);
            };

            private readonly It shouldHaveUpdatedTheEvidenceNoteWithStatusSubmitted = () =>
            {
                note.Status.Should().Be(NoteStatus.Submitted);
            };

            private readonly It shouldHaveRemovedTheEmptyTonnage = () =>
            {
                note.NoteTransferTonnage.Should().BeEmpty();
            };
        }

        public class SetNoteStatusRequestHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<SetNoteStatusRequest, Guid> handler;
            protected static Organisation organisation;
            protected static SetNoteStatusRequest request;
            protected static Guid result;
            protected static Note note;
            protected static Fixture fixture;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                    .WithExternalUserAccess();

                fixture = new Fixture();
                handler = Container.Resolve<IRequestHandler<SetNoteStatusRequest, Guid>>();

                return setup;
            }
        }
    }
}
