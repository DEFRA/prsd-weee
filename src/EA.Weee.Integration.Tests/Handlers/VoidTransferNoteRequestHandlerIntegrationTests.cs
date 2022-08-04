namespace EA.Weee.Integration.Tests.Handlers
{
    using System;
    using System.Threading.Tasks;
    using Autofac;
    using AutoFixture;
    using Base;
    using Builders;
    using Domain.Evidence;
    using Domain.Organisation;
    using FluentAssertions;
    using NUnit.Specifications;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using Requests.AatfEvidence;
    using NoteStatus = Domain.Evidence.NoteStatus;

    public class VoidTransferNoteRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIVoidTheNote : VoidTransferNoteRequestHandlerIntegrationTestBase
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
                        n.UpdateStatus(NoteStatus.Approved, UserId.ToString(), DateTime.UtcNow);
                    })
                    .Create();

                request = new VoidTransferNoteRequest(note.Id, "reason");
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                note = Query.GetEvidenceNoteById(note.Id);
            };

            private readonly It shouldHaveUpdatedTheTransferNoteToVoid = () =>
            {
                note.Status.Should().Be(NoteStatus.Void);
            };

            private readonly It shouldHaveCreatedANoteStatusHistory = () =>
            {
                var history = Query.GetLatestNoteStatusHistoryForNote(note.Id);
                history.ChangedById.Should().Be(UserId.ToString());
                history.ChangedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
                history.FromStatus.Should().Be(NoteStatus.Approved);
                history.ToStatus.Should().Be(NoteStatus.Void);
                history.Reason.Should().Be("reason");
            };
        }

        public class VoidTransferNoteRequestHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<VoidTransferNoteRequest, Guid> handler;
            protected static Organisation organisation;
            protected static VoidTransferNoteRequest request;
            protected static Guid result;
            protected static Note note;
            protected static Fixture fixture;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                    .WithInternalUserAccess(true);

                fixture = new Fixture();
                handler = Container.Resolve<IRequestHandler<VoidTransferNoteRequest, Guid>>();

                return setup;
            }
        }
    }
}
