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

    public class GetEvidenceNotesForTransferRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIGetNotesToTransferForAnOrganisation : GetEvidenceNotesForTransferRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, organisation.Id).Create();
                scheme = SchemeDbSetup.Init().WithOrganisation(organisation.Id).Create();

                var categories = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 2, 1),
                };

                note = EvidenceNoteDbSetup.Init().WithTonnages(categories).WithRecipient(scheme.Id).Create();

                notesSet.Add(note);
                
                request = new GetEvidenceNotesForTransferRequest(organisation.Id,
                    new List<Core.DataReturns.WeeeCategory>() { Core.DataReturns.WeeeCategory.AutomaticDispensers });
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldHaveReturnedTheEvidenceNote = () =>
            {
                result.Should().NotBeNull();
            };

            private readonly It shouldHaveReturnedTheEvidenceNoteWithExpectedPropertyValues = () =>
            {
                //ShouldMapToNote();
            };
        }

        [Component]
        public class WhenIGetANotesToTransferWhereUserIsNotAuthorised : GetEvidenceNotesForTransferRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                note = EvidenceNoteDbSetup.Init().Create();

                request = new GetEvidenceNotesForTransferRequest(Guid.NewGuid(),
                    new List<Core.DataReturns.WeeeCategory>());
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => handler.HandleAsync(request));
            };

            private readonly It shouldHaveCaughtArgumentException = ShouldThrowException<SecurityException>;
        }

        public class GetEvidenceNotesForTransferRequestHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<GetEvidenceNotesForTransferRequest, List<EvidenceNoteData>> handler;
            protected static Organisation organisation;
            protected static GetEvidenceNotesForTransferRequest request;
            protected static List<EvidenceNoteData> result;
            protected static Scheme scheme;  
            protected static Note note;
            protected static Fixture fixture;
            protected static List<Note> notesSet;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                    .WithExternalUserAccess();

                notesSet = new List<Note>();

                fixture = new Fixture();
                handler = Container.Resolve<IRequestHandler<GetEvidenceNotesForTransferRequest, List<EvidenceNoteData>>>();

                return setup;
            }
            //protected static void ShouldMapToNote()
            //{
            //    result.EndDate.Should().Be(note.EndDate);
            //    result.StartDate.Should().Be(note.StartDate);
            //    result.Reference.Should().Be(note.Reference);
            //    result.Protocol.ToInt().Should().Be(note.Protocol.ToInt());
            //    result.WasteType.ToInt().Should().Be(note.WasteType.ToInt());
            //    result.AatfData.Should().NotBeNull();
            //    result.AatfData.Id.Should().Be(note.Aatf.Id);
            //    result.SchemeData.Should().NotBeNull();
            //    result.SchemeData.Id.Should().Be(note.Recipient.Id);
            //    result.EvidenceTonnageData.Count.Should().Be(3);
            //    result.OrganisationData.Should().NotBeNull();
            //    result.OrganisationData.Id.Should().Be(note.Organisation.Id);
            //    ((int)result.Type).Should().Be(note.NoteType.Value);
            //    result.Id.Should().Be(note.Id);
            //    foreach (var noteTonnage in note.NoteTonnage)
            //    {
            //        result.EvidenceTonnageData.Should().Contain(n => n.Received.Equals(noteTonnage.Received) &&
            //                                                 n.Reused.Equals(noteTonnage.Reused) &&
            //                                                 ((int)n.CategoryId).Equals((int)noteTonnage.CategoryId));
            //    }
            //    result.RecipientOrganisationData.Should().NotBeNull();
            //    result.RecipientOrganisationData.Id.Should().Be(note.Recipient.OrganisationId);
            //}
        }
    }
}
