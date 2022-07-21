namespace EA.Weee.Integration.Tests.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Autofac;
    using AutoFixture;
    using Base;
    using Builders;
    using Core.Helpers;
    using Domain.Evidence;
    using Domain.Lookup;
    using Domain.Organisation;
    using FluentAssertions;
    using NUnit.Specifications;
    using Prsd.Core;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using Requests.Aatf;
    using Requests.AatfEvidence;
    using Protocol = Core.AatfEvidence.Protocol;
    using WasteType = Core.AatfEvidence.WasteType;

    public class EditEvidenceNoteRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIEditADraftEvidenceNote : EditEvidenceNoteHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                DefaultData();

                request = new EditEvidenceNoteRequest(existingNote.Id,
                    existingNote.Id,
                    updatedRecipient.Id,
                    DateTime.Now,
                    DateTime.Now.AddDays(1),
                    fixture.Create<WasteType>(),
                    fixture.Create<Protocol>(),
                    noteTonnages,
                    Core.AatfEvidence.NoteStatus.Draft,
                    existingNote.Id);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                updatedNote = Query.GetEvidenceNoteById(result);
            };

            private readonly It shouldHaveCreatedEvidenceNote = () =>
            {
                updatedNote.Should().NotBeNull();
            };

            private readonly It shouldHaveCreatedTheEvidenceNoteWithExpectedPropertyValues = () =>
            {
                ShouldMapToNote();
                updatedNote.Status.Should().Be(NoteStatus.Draft);
            };
        }

        [Component]
        public class WhenIEditADraftEvidenceNoteThatIsToBeSubmitted : EditEvidenceNoteHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                DefaultData();

                request = new EditEvidenceNoteRequest(existingNote.Id,
                    existingNote.Id,
                    updatedRecipient.Id,
                    DateTime.Now,
                    DateTime.Now.AddDays(1),
                    fixture.Create<WasteType>(),
                    fixture.Create<Protocol>(),
                    noteTonnages,
                    Core.AatfEvidence.NoteStatus.Submitted,
                    existingNote.Id);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                updatedNote = Query.GetEvidenceNoteById(result);
            };

            private readonly It shouldHaveCreateEvidenceNote = () =>
            {
                updatedNote.Should().NotBeNull();
            };

            private readonly It shouldHaveCreatedTheEvidenceNoteWithExpectedPropertyValues = () =>
            {
                ShouldMapToNote();
                updatedNote.Status.Should().Be(NoteStatus.Submitted);
            };

            private readonly It shouldHaveCreatedStatusChangeHistory = () =>
            {
                var history = Query.GetLatestNoteStatusHistoryForNote(updatedNote.Id);

                history.Should().NotBeNull();
                history.ChangedDate.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromSeconds(5));
                history.FromStatus.Should().Be(NoteStatus.Draft);
                history.ToStatus.Should().Be(NoteStatus.Submitted);
                history.ChangedById.Should().Be(UserId.ToString());
            };
        }

        public class EditEvidenceNoteHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<EditEvidenceNoteRequest, Guid> handler;
            protected static EditEvidenceNoteRequest request;
            protected static Organisation updatedRecipient;
            protected static Guid result;
            protected static Note existingNote;
            protected static Note updatedNote;
            protected static Fixture fixture;
            protected static List<TonnageValues> noteTonnages;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                    .WithExternalUserAccess();

                fixture = new Fixture();
                handler = Container.Resolve<IRequestHandler<EditEvidenceNoteRequest, Guid>>();

                return setup;
            }
            protected static void ShouldMapToNote()
            {
                updatedNote.CreatedById.Should().Be(UserId.ToString());
                updatedNote.Aatf.Should().Be(existingNote.Aatf);
                updatedNote.EndDate.Date.Should().Be(request.EndDate.Date);
                updatedNote.StartDate.Date.Should().Be(request.StartDate.Date);
                updatedNote.WasteType.ToInt().Should().Be(request.WasteType.ToInt());
                updatedNote.Protocol.ToInt().Should().Be(request.Protocol.ToInt());
                updatedNote.Recipient.Id.Should().Be(updatedRecipient.Id);
                updatedNote.Organisation.Should().Be(existingNote.Organisation);
                updatedNote.NoteType.Should().Be(NoteType.EvidenceNote);
                updatedNote.NoteTonnage.Count.Should().Be(request.TonnageValues.Count);
                foreach (var noteTonnage in request.TonnageValues)
                {
                    updatedNote.NoteTonnage.Should().Contain(n => n.Received.Equals(noteTonnage.FirstTonnage) &&
                                                                  n.Reused.Equals(noteTonnage.SecondTonnage) &&
                                                                  n.CategoryId.Equals((WeeeCategory)noteTonnage.CategoryId));
                }
            }
            protected static void DefaultData()
            {
                var existingTonnages = new List<NoteTonnage>()
                {
                    new NoteTonnage(fixture.Create<WeeeCategory>(), fixture.Create<decimal?>(),
                        fixture.Create<decimal?>()),
                    new NoteTonnage(fixture.Create<WeeeCategory>(), fixture.Create<decimal?>(),
                        fixture.Create<decimal?>()),
                    new NoteTonnage(fixture.Create<WeeeCategory>(), fixture.Create<decimal?>(),
                        fixture.Create<decimal?>())
                };

                existingNote = EvidenceNoteDbSetup.Init().WithTonnages(existingTonnages).Create();

                updatedRecipient = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(updatedRecipient.Id).Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, existingNote.OrganisationId).Create();

                noteTonnages = new List<TonnageValues>();

                foreach (var existingTonnage in existingTonnages)
                {
                    noteTonnages.Add(new TonnageValues(existingTonnage.Id, existingTonnage.CategoryId.ToInt(),
                        existingTonnage.Received, existingTonnage.Reused));
                }
            }
        }
    }
}
