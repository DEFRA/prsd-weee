﻿namespace EA.Weee.Integration.Tests.Handlers
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
    using FluentAssertions;
    using NUnit.Specifications;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using Requests.AatfEvidence;
    using NoteStatus = Domain.Evidence.NoteStatus;

    public class GetEvidenceNoteRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIGetADraftEvidenceNote : GetEvidenceNoteHandlerIntegrationTestBase
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

                request = new GetEvidenceNoteForAatfRequest(note.Id);
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
        public class WhenIGetASubmittedEvidenceNote : GetEvidenceNoteHandlerIntegrationTestBase
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
                
                request = new GetEvidenceNoteForAatfRequest(note.Id);
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
                result.Status.Should().Be(Core.AatfEvidence.NoteStatus.Submitted);
                result.SubmittedDate.Should().Be(note.NoteStatusHistory.First(n => n.ToStatus.Equals(NoteStatus.Submitted)).ChangedDate);
            };
        }

        [Component]
        public class WhenIGetAReturnedEvidenceNote : GetEvidenceNoteHandlerIntegrationTestBase
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
                        n.UpdateStatus(NoteStatus.Returned, UserId.ToString());
                    })
                    .Create();

                request = new GetEvidenceNoteForAatfRequest(note.Id);
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
                result.Status.Should().Be(Core.AatfEvidence.NoteStatus.Returned);
                result.ReturnedReason.Should().Be(note.NoteStatusHistory.First(n => n.ToStatus.Equals(NoteStatus.Returned)).Reason);
                result.ReturnedDate.Should().Be(note.NoteStatusHistory.First(n => n.ToStatus.Equals(NoteStatus.Returned)).ChangedDate);
            };
        }

        [Component]
        public class WhenIGetANoteWhereNoteDoesNotExist : GetEvidenceNoteHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                request = new GetEvidenceNoteForAatfRequest(Guid.NewGuid());
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => handler.HandleAsync(request));
            };

            private readonly It shouldHaveCaughtArgumentNullException = ShouldThrowException<ArgumentNullException>;
        }

        [Component]
        public class WhenIGetANotesWhereUserIsNotAuthorised : GetEvidenceNoteHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                note = EvidenceNoteDbSetup.Init().Create();

                request = new GetEvidenceNoteForAatfRequest(note.Id);
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => handler.HandleAsync(request));
            };

            private readonly It shouldHaveCaughtArgumentException = ShouldThrowException<SecurityException>;
        }

        public class GetEvidenceNoteHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<GetEvidenceNoteForAatfRequest, EvidenceNoteData> handler;
            protected static Organisation organisation;
            protected static GetEvidenceNoteForAatfRequest request;
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
                handler = Container.Resolve<IRequestHandler<GetEvidenceNoteForAatfRequest, EvidenceNoteData>>();

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
                result.RecipientId.Should().Be(note.Recipient.Id);
                result.Id.Should().Be(note.Id);
                result.ComplianceYear.Should().Be(note.ComplianceYear);
                foreach (var noteTonnage in note.NoteTonnage)
                {
                    result.EvidenceTonnageData.Should().Contain(n => n.Received.Equals(noteTonnage.Received) &&
                                                             n.Reused.Equals(noteTonnage.Reused) &&
                                                             ((int)n.CategoryId).Equals((int)noteTonnage.CategoryId));
                }
                result.RecipientOrganisationData.Should().NotBeNull();
                result.RecipientOrganisationData.Id.Should().Be(note.Recipient.OrganisationId);
            }
        }
    }
}
