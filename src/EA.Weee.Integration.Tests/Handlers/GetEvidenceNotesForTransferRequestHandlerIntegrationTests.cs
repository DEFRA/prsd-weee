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
            private static List<int> categories;

            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(organisation.Id).Create();

                var transferOrganisation = OrganisationDbSetup.Init().Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, transferOrganisation.Id).Create();
                SchemeDbSetup.Init().WithOrganisation(transferOrganisation.Id).Create();

                var transferOrganisationNonMatching = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(transferOrganisationNonMatching.Id).Create();

                // note to be included
                var categories1 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, null, null),
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 1, 2),
                    new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 10, 2),
                };
                notesSetToBeIncluded.Add(EvidenceNoteDbSetup.Init()
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Approved, UserId.ToString())
                    .WithTonnages(categories1).WithRecipient(transferOrganisation.Id).Create());

                // note to not be included no matching category
                var categories2 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 4, null),
                };
                notesSetToNotBeIncluded.Add(EvidenceNoteDbSetup.Init()
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Approved, UserId.ToString())
                    .WithTonnages(categories2).WithRecipient(transferOrganisation.Id).Create());

                // note to not be included not matching on scheme
                notesSetToNotBeIncluded.Add(EvidenceNoteDbSetup.Init().WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Approved, UserId.ToString())
                    .WithTonnages(new List<NoteTonnage>()).WithRecipient(transferOrganisationNonMatching.Id).Create());

                // note to be included
                var categories3 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 4, 8),
                    new NoteTonnage(WeeeCategory.MedicalDevices, 4, 8),
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 10, 8),
                };
                notesSetToBeIncluded.Add(EvidenceNoteDbSetup.Init()
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Approved, UserId.ToString())
                    .WithTonnages(categories3).WithRecipient(transferOrganisation.Id).Create());

                // note to not be included as submitted
                var categories4 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 1, null),
                };
                notesSetToNotBeIncluded.Add(EvidenceNoteDbSetup.Init()
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithTonnages(categories4).WithRecipient(transferOrganisation.Id).Create());

                // note to not be included as draft
                var categories5 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 2, null),
                };
                notesSetToNotBeIncluded.Add(EvidenceNoteDbSetup.Init()
                    .WithTonnages(categories5).WithRecipient(transferOrganisation.Id).Create());

                // note to not be included no tonnage entry
                var categories6 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, null, null),
                };
                notesSetToNotBeIncluded.Add(EvidenceNoteDbSetup.Init()
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Approved, UserId.ToString())
                    .WithTonnages(categories6).WithRecipient(transferOrganisation.Id).Create());

                // note to be not be included as in exclude list
                var categories7 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 4, 8),
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 10, 8),
                };
                var excludedNote = EvidenceNoteDbSetup.Init()
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Approved, UserId.ToString())
                    .WithTonnages(categories7).WithRecipient(transferOrganisation.Id).Create();
                notesSetToNotBeIncluded.Add(excludedNote);

                categories = new List<int>()
                {
                    Core.DataReturns.WeeeCategory.AutomaticDispensers.ToInt(),
                    Core.DataReturns.WeeeCategory.ElectricalAndElectronicTools.ToInt()
                };

                request = new GetEvidenceNotesForTransferRequest(transferOrganisation.Id,
                    categories,
                    notesSetToBeIncluded.ElementAt(0).ComplianceYear,
                    new List<Guid>()
                    {
                        excludedNote.Id
                    },
                    null);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldHaveReturnedCorrectEvidenceNotes = () =>
            {
                result.Results.Should().HaveCount(2);
                result.NoteCount.Should().Be(2);

                foreach (var evidenceNoteData in result.Results)
                {
                    notesSetToBeIncluded.First(n => n.Id.Equals(evidenceNoteData.Id)).Should().NotBeNull();

                    var refreshedNote = Query.GetEvidenceNoteById(evidenceNoteData.Id);

                    evidenceNoteData.ShouldMapToCutDownEvidenceNote(refreshedNote);

                    evidenceNoteData.TotalReceived.Should().Be(refreshedNote.NoteTonnage
                        .Where(nt => categories.Contains(nt.CategoryId.ToInt())).Sum(nt => nt.Received));

                    notesSetToNotBeIncluded.FirstOrDefault(n => n.Id == evidenceNoteData.Id).Should().BeNull();
                }
            };
        }

        [Component]
        public class WhenIGetNotesToTransferForAnOrganisationBySearchRef : GetEvidenceNotesForTransferRequestHandlerIntegrationTestBase
        {
            private static List<int> categories;

            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(organisation.Id).Create();

                var transferOrganisation = OrganisationDbSetup.Init().Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, transferOrganisation.Id).Create();
                SchemeDbSetup.Init().WithOrganisation(transferOrganisation.Id).Create();

                var transferOrganisationNonMatching = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(transferOrganisationNonMatching.Id).Create();

                // note to be included
                var categories1 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, null, null),
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 1, 2),
                    new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 10, 2),
                };
                notesSetToBeIncluded.Add(EvidenceNoteDbSetup.Init()
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Approved, UserId.ToString())
                    .WithTonnages(categories1).WithRecipient(transferOrganisation.Id).Create());

                // note to not be included no matching search ref
                var categories2 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 4, null),
                };
                notesSetToNotBeIncluded.Add(EvidenceNoteDbSetup.Init()
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Approved, UserId.ToString())
                    .WithTonnages(categories2).WithRecipient(transferOrganisation.Id).Create());

                categories = new List<int>()
                {
                    Core.DataReturns.WeeeCategory.AutomaticDispensers.ToInt(),
                    Core.DataReturns.WeeeCategory.ElectricalAndElectronicTools.ToInt()
                };

                request = new GetEvidenceNotesForTransferRequest(transferOrganisation.Id,
                    categories,
                    notesSetToBeIncluded.ElementAt(0).ComplianceYear,
                    new List<Guid>(),
                    notesSetToBeIncluded.ElementAt(0).Reference.ToString());
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldHaveReturnedCorrectEvidenceNote = () =>
            {
                result.Results.Should().HaveCount(1);
                result.NoteCount.Should().Be(1);

                foreach (var evidenceNoteData in result.Results)
                {
                    notesSetToBeIncluded.First(n => n.Id.Equals(evidenceNoteData.Id)).Should().NotBeNull();

                    var refreshedNote = Query.GetEvidenceNoteById(evidenceNoteData.Id);

                    evidenceNoteData.ShouldMapToCutDownEvidenceNote(refreshedNote);

                    evidenceNoteData.TotalReceived.Should().Be(refreshedNote.NoteTonnage
                        .Where(nt => categories.Contains(nt.CategoryId.ToInt())).Sum(nt => nt.Received));

                    notesSetToNotBeIncluded.FirstOrDefault(n => n.Id == evidenceNoteData.Id).Should().BeNull();
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

                request = new GetEvidenceNotesForTransferRequest(note.OrganisationId,
                    new List<int>() {1}, note.ComplianceYear, new List<Guid>());
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => handler.HandleAsync(request));
            };

            private readonly It shouldHaveCaughtArgumentException = ShouldThrowException<SecurityException>;
        }

        public class GetEvidenceNotesForTransferRequestHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<GetEvidenceNotesForTransferRequest, EvidenceNoteSearchDataResult> handler;
            protected static Organisation organisation;
            protected static GetEvidenceNotesForTransferRequest request;
            protected static EvidenceNoteSearchDataResult result;
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
                handler = Container.Resolve<IRequestHandler<GetEvidenceNotesForTransferRequest, EvidenceNoteSearchDataResult>>();

                return setup;
            }
        }
    }
}