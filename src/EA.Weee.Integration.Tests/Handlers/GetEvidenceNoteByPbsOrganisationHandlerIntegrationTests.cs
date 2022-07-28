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
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using Requests.AatfEvidence;
    using NoteStatus = Domain.Evidence.NoteStatus;

    public class GetEvidenceNoteByPbsOrganisationHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIGetADraftPBSEvidenceNote : GetEvidenceNoteByPbsOrganisationHandlerIntegrationTestBase
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

                var statuses = new List<Core.AatfEvidence.NoteStatus>()
                {
                    Core.AatfEvidence.NoteStatus.Draft
                };

                var noteTypeFilter = new List<Core.AatfEvidence.NoteType>()
                {
                    Core.AatfEvidence.NoteType.Evidence
                };

                note = EvidenceNoteDbSetup
                    .Init().WithTonnages(categories)
                    .WithRecipient(recipientOrganisation.Id)
                    .WithOrganisation(organisation.Id)
                    .WithComplianceYear(2022).Create();

                request = new GetEvidenceNoteByPbsOrganisationRequest(recipientOrganisation.Id, statuses, 2022, noteTypeFilter, false);
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
                result.Results.Should().OnlyContain(x => x.Status == Core.AatfEvidence.NoteStatus.Draft);
            };
        }

        [Component]
        public class WhenIGetASubmittedPBSEvidenceNote : GetEvidenceNoteByPbsOrganisationHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                recipientOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation.Id).Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, recipientOrganisation.Id).Create();

                var statuses = new List<Core.AatfEvidence.NoteStatus>()
                {
                    Core.AatfEvidence.NoteStatus.Submitted
                };

                var noteTypeFilter = new List<Core.AatfEvidence.NoteType>()
                {
                    Core.AatfEvidence.NoteType.Evidence
                };

                note = EvidenceNoteDbSetup
                    .Init()
                    .WithRecipient(recipientOrganisation.Id)
                    .WithOrganisation(organisation.Id)
                    .WithComplianceYear(2022)
                    .WithStatus(NoteStatus.Submitted, UserId.ToString()).Create();

                request = new GetEvidenceNoteByPbsOrganisationRequest(recipientOrganisation.Id, statuses, 2022, noteTypeFilter, false);
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
                result.Results.Should().OnlyContain(x => x.Status == Core.AatfEvidence.NoteStatus.Submitted);
            };
        }

        [Component]
        public class WhenIGetOneReturnedPBSEvidenceNote : GetEvidenceNoteByPbsOrganisationHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                recipientOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation.Id).Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, recipientOrganisation.Id).Create();

                var statuses = new List<Core.AatfEvidence.NoteStatus>()
                {
                    Core.AatfEvidence.NoteStatus.Returned
                };

                var noteTypeFilter = new List<Core.AatfEvidence.NoteType>()
                {
                    Core.AatfEvidence.NoteType.Evidence
                };

                note = EvidenceNoteDbSetup
                    .Init()
                    .WithRecipient(recipientOrganisation.Id)
                    .WithOrganisation(organisation.Id)
                    .WithComplianceYear(2022)
                    .WithStatus(NoteStatus.Returned, UserId.ToString()).Create();

                request = new GetEvidenceNoteByPbsOrganisationRequest(recipientOrganisation.Id, statuses, 2022, noteTypeFilter, false);
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
                result.Results.Should().OnlyContain(x => x.Status == Core.AatfEvidence.NoteStatus.Returned);
            };
        }

        [Component]
        public class WhenIGetOneRejectedPBSEvidenceNote : GetEvidenceNoteByPbsOrganisationHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                recipientOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation.Id).Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, recipientOrganisation.Id).Create();

                var statuses = new List<Core.AatfEvidence.NoteStatus>()
                {
                    Core.AatfEvidence.NoteStatus.Rejected
                };

                var noteTypeFilter = new List<Core.AatfEvidence.NoteType>()
                {
                    Core.AatfEvidence.NoteType.Evidence
                };

                note = EvidenceNoteDbSetup
                    .Init()
                    .WithRecipient(recipientOrganisation.Id)
                    .WithOrganisation(organisation.Id)
                    .WithComplianceYear(2022)
                    .WithStatus(NoteStatus.Rejected, UserId.ToString()).Create();

                request = new GetEvidenceNoteByPbsOrganisationRequest(recipientOrganisation.Id, statuses, 2022, noteTypeFilter, false);
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
                result.Results.Should().OnlyContain(x => x.Status == Core.AatfEvidence.NoteStatus.Rejected);
            };
        }

        [Component]
        public class WhenIGetOnePBSNoteWhereNoteDoesNotExist : GetEvidenceNoteByPbsOrganisationHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();
                organisation = OrganisationDbSetup.Init().Create();
            };

            private readonly Because of = () =>
            {
                CatchException(() => new GetEvidenceNoteByPbsOrganisationRequest(organisation.Id, null, 2022, new List<Core.AatfEvidence.NoteType>(), false));
            };

            private readonly It shouldHaveCaughtArgumentNullException = ShouldThrowException<ArgumentNullException>;
        }

        [Component]
        public class WhenIGetOnePBSNotesWhereUserIsNotAuthorised : GetEvidenceNoteByPbsOrganisationHandlerIntegrationTestBase
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

                var statuses = new List<Core.AatfEvidence.NoteStatus>()
                {
                    Core.AatfEvidence.NoteStatus.Draft
                };

                var noteTypeFilter = new List<Core.AatfEvidence.NoteType>()
                {
                    Core.AatfEvidence.NoteType.Transfer
                };

                note = EvidenceNoteDbSetup
                    .Init().WithTonnages(categories)
                    .WithRecipient(recipientOrganisation.Id)
                    .WithOrganisation(organisation.Id).Create();

                request = new GetEvidenceNoteByPbsOrganisationRequest(organisation.Id, statuses, 2022, noteTypeFilter, false);
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => handler.HandleAsync(request));
            };

            private readonly It shouldHaveCaughtArgumentException = ShouldThrowException<SecurityException>;
        }

        public class GetEvidenceNoteByPbsOrganisationHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<GetEvidenceNoteByPbsOrganisationRequest, EvidenceNoteSearchDataResult> handler;
            protected static Organisation organisation;
            protected static Organisation recipientOrganisation;
            protected static GetEvidenceNoteByPbsOrganisationRequest request;
            protected static EvidenceNoteSearchDataResult result;
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
                handler = Container.Resolve<IRequestHandler<GetEvidenceNoteByPbsOrganisationRequest, EvidenceNoteSearchDataResult>>();

                return setup;
            }
            protected static void ShouldMapToNote()
            {
                result.Results.FirstOrDefault().EndDate.Should().Be(note.EndDate);
                result.Results.FirstOrDefault().StartDate.Should().Be(note.StartDate);
                result.Results.FirstOrDefault().Reference.Should().Be(note.Reference);
                result.Results.FirstOrDefault().Protocol.ToInt().Should().Be(note.Protocol.ToInt());
                result.Results.FirstOrDefault().WasteType.ToInt().Should().Be(note.WasteType.ToInt());
                result.Results.FirstOrDefault().AatfData.Should().NotBeNull();
                result.Results.FirstOrDefault().AatfData.Id.Should().Be(note.Aatf.Id);
                result.Results.FirstOrDefault().RecipientSchemeData.Should().NotBeNull();
                var recipientScheme = Query.GetSchemeByOrganisationId(recipientOrganisation.Id);
                result.Results.FirstOrDefault().RecipientSchemeData.Id.Should().Be(recipientScheme.Id);
                result.Results.FirstOrDefault().OrganisationData.Should().NotBeNull();
                result.Results.FirstOrDefault().OrganisationData.Id.Should().Be(note.Organisation.Id);
                ((int)result.Results.FirstOrDefault().Type).Should().Be(note.NoteType.Value);
                result.Results.FirstOrDefault().Id.Should().Be(note.Id);
                result.Results.FirstOrDefault().ComplianceYear.Should().Be(note.ComplianceYear);
                result.Results.FirstOrDefault().RecipientOrganisationData.Should().NotBeNull();
                result.Results.FirstOrDefault().RecipientOrganisationData.Id.Should().Be(note.Recipient.Id);
            }
        }
    }
}
