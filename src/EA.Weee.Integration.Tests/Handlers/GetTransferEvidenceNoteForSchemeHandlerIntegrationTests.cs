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
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NUnit.Specifications;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using Requests.AatfEvidence;
    using NoteStatus = Domain.Evidence.NoteStatus;

    public class GetTransferEvidenceNoteForSchemeHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIGetADraftTransferEvidenceNote : GetTransferEvidenceNoteForSchemeHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, organisation.Id).Create();

                var noteTonnages = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 2, 1)
                };

                note = EvidenceNoteDbSetup.Init().WithTonnages(noteTonnages).Create();
                
                var noteTransferTonnage = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(note.NoteTonnage.ElementAt(0).Id, 1, null)
                };

                TransferEvidenceNoteDbSetup.Init().WithTonnages(noteTransferTonnage).Create();

                request = new GetTransferEvidenceNoteForSchemeRequest(note.Id);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                note = Query.GetEvidenceNoteById(note.Id);
            };

            private readonly It shouldHaveReturnedTheTransferEvidenceNote = () =>
            {
                result.Should().NotBeNull();
            };

            private readonly It shouldHaveReturnedTheTransferEvidenceNoteWithExpectedPropertyValues = () =>
            {
                ShouldMapToNote();
                result.Status.Should().Be(EA.Weee.Core.AatfEvidence.NoteStatus.Draft);
            };
        }

        [Component]
        [Ignore("Put back in when submission is in place")]
        public class WhenIGetASubmittedTransferEvidenceNote : GetTransferEvidenceNoteForSchemeHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, organisation.Id).Create();

                var noteTonnages = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 2, 1)
                };

                note = EvidenceNoteDbSetup.Init().WithTonnages(noteTonnages).Create();

                var noteTransferTonnage = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(note.NoteTonnage.ElementAt(0).Id, 1, null)
                };

                TransferEvidenceNoteDbSetup.Init().WithTonnages(noteTransferTonnage).WithStatus(NoteStatus.Submitted, UserId.ToString()).Create();

                request = new GetTransferEvidenceNoteForSchemeRequest(note.Id);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                note = Query.GetEvidenceNoteById(note.Id);
            };

            private readonly It shouldHaveCreatedTheTransferEvidenceNote = () =>
            {
                result.Should().NotBeNull();
            };

            private readonly It shouldHaveCreatedTheTransferNoteWithExpectedPropertyValues = () =>
            {
                ShouldMapToNote();
                result.Status.Should().Be(EA.Weee.Core.AatfEvidence.NoteStatus.Submitted);
                result.SubmittedDate.Should().Be(note.NoteStatusHistory.First(n => n.ToStatus.Equals(NoteStatus.Submitted)).ChangedDate);
            };
        }

        [Component]
        public class WhenIGetATransferNoteWhereNoteDoesNotExist : GetTransferEvidenceNoteForSchemeHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                request = new GetTransferEvidenceNoteForSchemeRequest(Guid.NewGuid());
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => handler.HandleAsync(request));
            };

            private readonly It shouldHaveCaughtArgumentNullException = ShouldThrowException<ArgumentNullException>;
        }

        [Component]
        public class WhenIGetANotesWhereUserIsNotAuthorised : GetTransferEvidenceNoteForSchemeHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                note = TransferEvidenceNoteDbSetup.Init().Create();

                request = new GetTransferEvidenceNoteForSchemeRequest(note.Id);
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => handler.HandleAsync(request));
            };

            private readonly It shouldHaveCaughtArgumentException = ShouldThrowException<SecurityException>;
        }

        public class GetTransferEvidenceNoteForSchemeHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<GetTransferEvidenceNoteForSchemeRequest, TransferEvidenceNoteData> handler;
            protected static Organisation organisation;
            protected static GetTransferEvidenceNoteForSchemeRequest request;
            protected static TransferEvidenceNoteData result;
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
                handler = Container.Resolve<IRequestHandler<GetTransferEvidenceNoteForSchemeRequest, TransferEvidenceNoteData>>();

                return setup;
            }
            protected static void ShouldMapToNote()
            {
                result.Reference.Should().Be(note.Reference);
                result.Status.ToInt().Should().Be(note.Status.Value);
                ((int)result.Type).Should().Be(note.NoteType.Value);
                result.Id.Should().Be(note.Id);
            }
        }
    }
}
