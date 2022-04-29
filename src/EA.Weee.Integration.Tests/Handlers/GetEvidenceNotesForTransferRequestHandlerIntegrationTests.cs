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

                var scheme2 = SchemeDbSetup.Init().WithOrganisation(organisation.Id).Create();

                var categories1 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, null, null),
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 1, 2),
                };

                // note to be included
                notesSetToBeIncluded.Add(EvidenceNoteDbSetup.Init()
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Approved, UserId.ToString())
                    .WithTonnages(categories1).WithRecipient(scheme.Id).Create());

                var categories2 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, null, null),
                };

                // not to not be included no matching category
                notesSetToNotBeIncluded.Add(EvidenceNoteDbSetup.Init()
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Approved, UserId.ToString())
                    .WithTonnages(categories2).WithRecipient(scheme.Id).Create());

                // not to be included not matching on scheme
                notesSetToNotBeIncluded.Add(EvidenceNoteDbSetup.Init().WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Approved, UserId.ToString())
                    .WithTonnages(new List<NoteTonnage>()).WithRecipient(scheme2.Id).Create());

                var categories3 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 4, 8),
                };

                // to be included
                notesSetToBeIncluded.Add(EvidenceNoteDbSetup.Init()
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Approved, UserId.ToString())
                    .WithTonnages(categories3).WithRecipient(scheme.Id).Create());

                var categories4 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, null, null),
                };

                // not to be included as submitted
                notesSetToNotBeIncluded.Add(EvidenceNoteDbSetup.Init()
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithTonnages(categories4).WithRecipient(scheme.Id).Create());

                var categories5 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, null, null),
                };

                // not to be included as draft
                notesSetToNotBeIncluded.Add(EvidenceNoteDbSetup.Init()
                    .WithTonnages(categories5).WithRecipient(scheme.Id).Create());

                request = new GetEvidenceNotesForTransferRequest(organisation.Id,
                    new List<int>()
                    {
                        Core.DataReturns.WeeeCategory.AutomaticDispensers.ToInt(),
                        Core.DataReturns.WeeeCategory.ElectricalAndElectronicTools.ToInt()
                    });
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldHaveReturnedCorrectEvidenceNotes = () =>
            {
                result.Should().HaveCount(2);
                foreach (var evidenceNoteData in result)
                {
                    notesSetToBeIncluded.First(n => n.Id.Equals(evidenceNoteData.Id)).Should().NotBeNull();

                    var refreshedNote = Query.GetEvidenceNoteById(evidenceNoteData.Id);

                    evidenceNoteData.ShouldMapToNote(refreshedNote);
                }
            };
        }

        [Component]
        public class WhenIGetNotesToTransferWhereUserIsNotAuthorised : GetEvidenceNotesForTransferRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                note = EvidenceNoteDbSetup.Init().Create();

                request = new GetEvidenceNotesForTransferRequest(Guid.NewGuid(),
                    new List<int>() {1});
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
            protected static List<Note> notesSetToBeIncluded;
            protected static List<Note> notesSetToNotBeIncluded;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                    .WithExternalUserAccess();

                notesSetToBeIncluded = new List<Note>();
                notesSetToNotBeIncluded = new List<Note>();

                fixture = new Fixture();
                handler = Container.Resolve<IRequestHandler<GetEvidenceNotesForTransferRequest, List<EvidenceNoteData>>>();

                return setup;
            }
        }
    }
}
