namespace EA.Weee.Integration.Tests.Handlers
{
    using Autofac;
    using AutoFixture;
    using Base;
    using Builders;
    using Core.Shared;
    using Domain.Evidence;
    using Domain.Organisation;
    using EA.Weee.Requests.Shared;
    using FluentAssertions;
    using NUnit.Specifications;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using System;
    using System.Threading.Tasks;
    using NoteStatus = Domain.Evidence.NoteStatus;

    public class VoidNoteRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIVoidTheNote : VoidNoteRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(organisation.Id).Create();
                var recipientOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation.Id).Create();

                note = TransferEvidenceNoteDbSetup.Init()
                    .WithRecipient(recipientOrganisation.Id)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatus.Submitted, UserId.ToString(), DateTime.UtcNow);
                        n.UpdateStatus(NoteStatus.Approved, UserId.ToString(), DateTime.UtcNow);
                    })
                    .Create();

                request = new VoidNoteRequest(note.Id, "reason");
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

        public class VoidNoteRequestHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<VoidNoteRequest, Guid> handler;
            protected static Organisation organisation;
            protected static VoidNoteRequest request;
            protected static Guid result;
            protected static Note note;
            protected static Fixture fixture;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                    .WithInternalUserAccess();

                Query.SetupUserWithRole(UserId.ToString(), "Administrator", CompetentAuthority.England);

                fixture = new Fixture();
                handler = Container.Resolve<IRequestHandler<VoidNoteRequest, Guid>>();

                return setup;
            }
        }
    }
}
