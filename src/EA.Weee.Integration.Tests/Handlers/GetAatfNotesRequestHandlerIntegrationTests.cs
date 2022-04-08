namespace EA.Weee.Integration.Tests.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Autofac;
    using Base;
    using Builders;
    using Domain.Organisation;
    using Domain.Scheme;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.Evidence;
    using EA.Weee.Domain.User;
    using EA.Weee.Requests.AatfEvidence;
    using FluentAssertions;
    using NUnit.Specifications;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using Requests.Scheme;

    public class GetAatfNotesRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIGetEvidenceNoteForOrganisationWhichExternalUserDoesNotHaveAccessTo : GetAatfNotesRequestHandlerTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                aatf = AatfDbSetup.Init().Create();
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => handler.HandleAsync(new GetAatfNotesRequest(Guid.NewGuid(), aatf.Id)));
            };

            private readonly It shouldHaveCaughtSecurityException = ShouldThrowException<SecurityException>;
        }

        [Component]
        public class WhenIGetAListOfEvidenceNoteDataAsNotesAreSubmittedStatus : GetAatfNotesRequestHandlerTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                 organisation = OrganisationDbSetup.Init()
                .Create();

                aatf = AatfDbSetup.Init()
                .WithOrganisation(organisation)
                .Create();

                OrganisationUserDbSetup.Init()
                .WithUserIdAndOrganisationId(UserId, organisation.Id)
                .WithStatus(UserStatus.Active)
                .Create();

                var evidence1 = EvidenceNoteDbSetup.Init()
                .WithOrganisation(organisation.Id)
                .WithAatf(aatf.Id)
                .Create();

                var evidence2 = EvidenceNoteDbSetup.Init()
                .WithOrganisation(organisation.Id)
                .WithAatf(aatf.Id)
                .Create();

                var evidence3 = EvidenceNoteDbSetup.Init()
                .WithOrganisation(organisation.Id)
                .WithAatf(aatf.Id)
                .Create();

                notesSet.Add(evidence1);
                notesSet.Add(evidence2);
                notesSet.Add(evidence3);
            };

            private readonly Because of = () =>
            {
                evidenceNoteData = Task.Run(async () => await handler.HandleAsync(new GetAatfNotesRequest(organisation.Id, aatf.Id))).Result;
            };

            private readonly It shouldReturnListOfEvidenceNotes = () =>
            {
                evidenceNoteData.Should().NotBeNull();
            };

            private readonly It shouldHaveExpectedResultsCount = () =>
            {
                evidenceNoteData.Should().HaveCount(notesSet.Count);
            };

            private readonly It shouldHaveExpectedReferenceData = () =>
            {
                foreach (var note1 in notesSet)
                {
                    var evidenceNote = evidenceNoteData.FirstOrDefault(n => n.Id.Equals(note1.Id));
                    evidenceNote.Should().NotBeNull();
                }
            };

            private readonly It shouldHaveNotesInExpectedOrder = () =>
            {
                evidenceNoteData.Should().BeInDescendingOrder(e => e.Reference);
            };
        }

        [Component]
        public class WhenIGetNoEvidenceNoteDataAsNotesAreSubmittedStatus : GetAatfNotesRequestHandlerTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init()
               .Create();

                aatf = AatfDbSetup.Init()
                .WithOrganisation(organisation)
                .Create();

                OrganisationUserDbSetup.Init()
                .WithUserIdAndOrganisationId(UserId, organisation.Id)
                .WithStatus(UserStatus.Active)
                .Create();

                var evidence1 = EvidenceNoteDbSetup.Init()
                .WithOrganisation(organisation.Id)
                .WithAatf(aatf.Id)
                .WithStatus(Domain.Evidence.NoteStatus.Submitted, UserId.ToString())
                .Create();

                var evidence2 = EvidenceNoteDbSetup.Init()
                .WithOrganisation(organisation.Id)
                .WithAatf(aatf.Id)
                .WithStatus(Domain.Evidence.NoteStatus.Submitted, UserId.ToString())
               .Create();
            };

            private readonly Because of = () =>
            {
                evidenceNoteData = Task.Run(async () => await handler.HandleAsync(new GetAatfNotesRequest(organisation.Id, aatf.Id))).Result;
            };

            private readonly It shouldReturnNoData = () =>
            {
                evidenceNoteData.Should().BeNullOrEmpty();
            };
        }

        public class GetAatfNotesRequestHandlerTestBase : WeeeContextSpecification
        {
            protected static List<EvidenceNoteData> evidenceNoteData;
            protected static List<Note> notesSet;
            protected static IRequestHandler<GetAatfNotesRequest, List<EvidenceNoteData>> handler;
            protected static Organisation organisation;
            protected static Scheme scheme;
            protected static Aatf aatf;
            protected static Note note;

            public static void LocalSetup()
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings(resetDb: true)
                    .WithExternalUserAccess();

                notesSet = new List<Note>();
                handler = Container.Resolve<IRequestHandler<GetAatfNotesRequest, List<EvidenceNoteData>>>();
            }
        }
    }
}
