namespace EA.Weee.Integration.Tests.Handlers
{
    using System;
    using System.Collections.Generic;
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
    using FluentAssertions;
    using NUnit.Specifications;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using Requests.AatfEvidence;
    using NoteStatus = Domain.Evidence.NoteStatus;

    public class GetEvidenceNoteForSchemeHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIGetADraftEvidenceNote : GetEvidenceNoteForSchemeHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, organisation.Id).Create();

                var categories = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 2, 1),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 0, 0)
                };

                note = EvidenceNoteDbSetup.Init().WithTonnages(categories).WithOrganisation(organisation.Id).Create();

                request = new GetEvidenceNoteRequest(note.Id);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                note = Query.GetEvidenceNoteById(note.Id);
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
        public class WhenIGetASubmittedEvidenceNote : GetEvidenceNoteForSchemeHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, organisation.Id).Create();

                var categories = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 2, 1),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 0, 0)
                };

                note = EvidenceNoteDbSetup.Init().WithTonnages(categories)
                    .WithOrganisation(organisation.Id)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatus.Submitted, UserId.ToString());
                    })
                    .Create();
                
                request = new GetEvidenceNoteRequest(note.Id);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                note = Query.GetEvidenceNoteById(note.Id);
            };

            private readonly It shouldHaveCreatedEvidenceNote = () =>
            {
                note.Should().NotBeNull();
            };

            private readonly It shouldHaveCreatedTheEvidenceNoteWithExpectedPropertyValues = () =>
            {
                ShouldMapToNote();
                note.Status.Should().Be(NoteStatus.Submitted);
            };
        }

        [Component]
        public class WhenIGetANoteWhereNoteDoesNotExist : GetEvidenceNoteForSchemeHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                request = new GetEvidenceNoteRequest(Guid.NewGuid());
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => handler.HandleAsync(request));
            };

            private readonly It shouldHaveCaughtArgumentNullException = ShouldThrowException<ArgumentNullException>;
        }

        [Component]
        public class WhenIGetANotesWhereUserIsNotAuthorised : GetEvidenceNoteForSchemeHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                note = EvidenceNoteDbSetup.Init().Create();

                request = new GetEvidenceNoteRequest(note.Id);
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => handler.HandleAsync(request));
            };

            private readonly It shouldHaveCaughtArgumentException = ShouldThrowException<SecurityException>;
        }

        public class GetEvidenceNoteForSchemeHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<GetEvidenceNoteRequest, EvidenceNoteData> handler;
            protected static Organisation organisation;
            protected static GetEvidenceNoteRequest request;
            protected static EvidenceNoteData result;
            protected static Note note;
            protected static Fixture fixture;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                    .WithExternalUserAccess();

                fixture = new Fixture();
                handler = Container.Resolve<IRequestHandler<GetEvidenceNoteRequest, EvidenceNoteData>>();

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
                result.SchemeData.Should().NotBeNull();
                result.SchemeData.Id.Should().Be(note.Recipient.Id);
                result.EvidenceTonnageData.Count.Should().Be(3);
                result.OrganisationData.Should().NotBeNull();
                result.OrganisationData.Id.Should().Be(note.Organisation.Id);
                ((int)result.Type).Should().Be(note.NoteType.Value);
                result.Id.Should().Be(note.Id);
                foreach (var noteTonnage in note.NoteTonnage)
                {
                    result.EvidenceTonnageData.Should().Contain(n => n.Received.Equals(noteTonnage.Received) &&
                                                             n.Reused.Equals(noteTonnage.Reused) &&
                                                             ((int)n.CategoryId).Equals((int)noteTonnage.CategoryId));
                }
            }
        }
    }
}
