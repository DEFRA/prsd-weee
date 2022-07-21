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
    using Core.Helpers;
    using Domain.AatfReturn;
    using Domain.Evidence;
    using Domain.Lookup;
    using Domain.Organisation;
    using Domain.Scheme;
    using FluentAssertions;
    using NUnit.Specifications;
    using Prsd.Core;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using Requests.Aatf;
    using Requests.AatfEvidence;
    using Protocol = Core.AatfEvidence.Protocol;
    using WasteType = Core.AatfEvidence.WasteType;

    public class CreateEvidenceNoteRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenICreateADraftEvidenceNote : CreateEvidenceNoteHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                var builder = LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                aatf = AatfDbSetup.Init().WithOrganisation(organisation.Id).Create();
                
                recipientOrganisation = OrganisationDbSetup.Init().Create();
                var recipientScheme = SchemeDbSetup.Init().WithOrganisation(recipientOrganisation.Id).Create();

                OrganisationUserDbSetup
                    .Init()
                    .WithUserIdAndOrganisationId(UserId, organisation.Id).Create();
               
                var categories = new List<TonnageValues>()
                {
                    new TonnageValues(Guid.Empty, (int)WeeeCategory.AutomaticDispensers, 2, 1),
                    new TonnageValues(Guid.Empty, (int)WeeeCategory.ConsumerEquipment, null, null),
                    new TonnageValues(Guid.Empty, (int)WeeeCategory.GasDischargeLampsAndLedLightSources, 0, 0)
                };

                request = new CreateEvidenceNoteRequest(organisation.Id,
                    aatf.Id,
                    recipientScheme.Id,
                    DateTime.Now,
                    DateTime.Now.AddDays(1),
                    fixture.Create<WasteType>(),
                    fixture.Create<Protocol>(),
                    categories.ToList(),
                    Core.AatfEvidence.NoteStatus.Draft,
                    Guid.Empty);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                note = Query.GetEvidenceNoteById(result);
            };

            private readonly It shouldHaveCreatedEvidenceNote = () =>
            {
                note.Should().NotBeNull();
            };

            private readonly It shouldHaveCreatedTheEvidenceNoteWithExpectedPropertyValues = () =>
            {
                ShouldMapToNote();
                note.Status.Should().Be(NoteStatus.Draft);
            };
        }

        [Component]
        public class WhenICreateASubmittedEvidenceNote : CreateEvidenceNoteHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                var builder = LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                aatf = AatfDbSetup.Init().WithOrganisation(organisation.Id).Create();

                recipientOrganisation = OrganisationDbSetup.Init().Create();
                var recipientScheme = SchemeDbSetup.Init().WithOrganisation(recipientOrganisation.Id).Create();

                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, organisation.Id).Create();

                var categories = new List<TonnageValues>()
                {
                    new TonnageValues(Guid.Empty, (int)WeeeCategory.ITAndTelecommsEquipment, null, 999),
                    new TonnageValues(Guid.Empty, (int)WeeeCategory.MedicalDevices, 20, null),
                    new TonnageValues(Guid.Empty, (int)WeeeCategory.ConsumerEquipment, 10, 0)
                };

                request = new CreateEvidenceNoteRequest(organisation.Id,
                    aatf.Id,
                    recipientScheme.Id,
                    DateTime.Now,
                    DateTime.Now.AddDays(1),
                    fixture.Create<WasteType>(),
                    fixture.Create<Protocol>(),
                    categories.ToList(),
                    Core.AatfEvidence.NoteStatus.Submitted,
                    Guid.Empty);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                note = Query.GetEvidenceNoteById(result);
            };

            private readonly It shouldHaveCreateEvidenceNote = () =>
            {
                note.Should().NotBeNull();
            };

            private readonly It shouldHaveCreatedTheEvidenceNoteWithExpectedPropertyValues = () =>
            {
                ShouldMapToNote();
                note.Status.Should().Be(NoteStatus.Submitted);
            };

            private readonly It shouldHaveCreatedStatusChangeHistory = () =>
            {
                var history = Query.GetLatestNoteStatusHistoryForNote(note.Id);

                history.Should().NotBeNull();
                history.ChangedDate.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromSeconds(5));
                history.FromStatus.Should().Be(NoteStatus.Draft);
                history.ToStatus.Should().Be(NoteStatus.Submitted);
                history.ChangedById.Should().Be(UserId.ToString());
            };
        }

        public class CreateEvidenceNoteHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<CreateEvidenceNoteRequest, Guid> handler;
            protected static Organisation organisation;
            protected static Aatf aatf;
            protected static CreateEvidenceNoteRequest request;
            protected static Organisation recipientOrganisation;
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
                handler = Container.Resolve<IRequestHandler<CreateEvidenceNoteRequest, Guid>>();

                return setup;
            }
            protected static void ShouldMapToNote()
            {
                note.CreatedById.Should().Be(UserId.ToString());
                note.Aatf.Should().Be(aatf);
                note.EndDate.Date.Should().Be(request.EndDate.Date);
                note.StartDate.Date.Should().Be(request.StartDate.Date);
                note.WasteType.ToInt().Should().Be(request.WasteType.ToInt());
                note.Protocol.ToInt().Should().Be(request.Protocol.ToInt());
                note.Recipient.Should().Be(recipientOrganisation);
                note.Reference.Should().BeGreaterThan(0);
                note.CreatedDate.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromSeconds(10));
                note.Organisation.Should().Be(organisation);
                note.NoteType.Should().Be(NoteType.EvidenceNote);
                note.NoteTonnage.Count.Should().Be(request.TonnageValues.Count);
                foreach (var noteTonnage in request.TonnageValues)
                {
                    note.NoteTonnage.Should().Contain(n => n.Received.Equals(noteTonnage.FirstTonnage) &&
                                                           n.Reused.Equals(noteTonnage.SecondTonnage) &&
                                                           n.CategoryId.Equals((WeeeCategory)noteTonnage.CategoryId));
                }
            }
        }
    }
}
