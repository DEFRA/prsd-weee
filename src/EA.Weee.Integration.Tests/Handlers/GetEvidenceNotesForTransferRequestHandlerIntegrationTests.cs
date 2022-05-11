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
                    .WithTonnages(categories1).WithRecipient(scheme.Id).Create());

                // note to not be included no matching category
                var categories2 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 4, null),
                };
                notesSetToNotBeIncluded.Add(EvidenceNoteDbSetup.Init()
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Approved, UserId.ToString())
                    .WithTonnages(categories2).WithRecipient(scheme.Id).Create());

                // note to not be included not matching on scheme
                notesSetToNotBeIncluded.Add(EvidenceNoteDbSetup.Init().WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Approved, UserId.ToString())
                    .WithTonnages(new List<NoteTonnage>()).WithRecipient(scheme2.Id).Create());

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
                    .WithTonnages(categories3).WithRecipient(scheme.Id).Create());

                // note to not be included as submitted
                var categories4 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 1, null),
                };
                notesSetToNotBeIncluded.Add(EvidenceNoteDbSetup.Init()
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithTonnages(categories4).WithRecipient(scheme.Id).Create());

                // note to not be included as draft
                var categories5 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 2, null),
                };
                notesSetToNotBeIncluded.Add(EvidenceNoteDbSetup.Init()
                    .WithTonnages(categories5).WithRecipient(scheme.Id).Create());

                // note to not be included no tonnage entry
                var categories6 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, null, null),
                };
                notesSetToNotBeIncluded.Add(EvidenceNoteDbSetup.Init()
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Approved, UserId.ToString())
                    .WithTonnages(categories6).WithRecipient(scheme.Id).Create());

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

                // first note has null automatic dispensers so this should not be returned as tonnage
                // first note has category ITAndTelecommsEquipment that is not in the category list so also shouldnt be in the tonnage data
                var note1 = result.First(r => r.Id.Equals(notesSetToBeIncluded.ElementAt(0).Id));
                note1.EvidenceTonnageData.Count.Should().Be(1);
                note1.EvidenceTonnageData.ElementAt(0).Received.Should().Be(1);
                note1.EvidenceTonnageData.ElementAt(0).Reused.Should().Be(2);
                note1.EvidenceTonnageData.ElementAt(0).Id.Should().NotBe(Guid.Empty);
                note1.EvidenceTonnageData.ElementAt(0).CategoryId.Should()
                    .Be(Core.DataReturns.WeeeCategory.ElectricalAndElectronicTools);

                // second note has category MedicalDevices that is not in the category list so also shouldnt be in the tonnage data
                var note2 = result.First(r => r.Id.Equals(notesSetToBeIncluded.ElementAt(1).Id));
                note2.EvidenceTonnageData.Count.Should().Be(2);
                var categoryTonnage = note2.EvidenceTonnageData.First(e =>
                    e.CategoryId.Equals(Core.DataReturns.WeeeCategory.ElectricalAndElectronicTools));
                categoryTonnage.Received.Should().Be(10);
                categoryTonnage.Reused.Should().Be(8);
                categoryTonnage.Id.Should().NotBe(Guid.Empty);
                categoryTonnage = note2.EvidenceTonnageData.First(e =>
                    e.CategoryId.Equals(Core.DataReturns.WeeeCategory.AutomaticDispensers));
                categoryTonnage.Received.Should().Be(4);
                categoryTonnage.Reused.Should().Be(8);
                categoryTonnage.Id.Should().NotBe(Guid.Empty);
            };
        }

        [Component]
        public class WhenIGetNotesToTransferForAnOrganisationThatHasNoteList : GetEvidenceNotesForTransferRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, organisation.Id).Create();
                scheme = SchemeDbSetup.Init().WithOrganisation(organisation.Id).Create();

                // note to be included
                var categories1 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 1, null)
                };
                notesSetToBeIncluded.Add(EvidenceNoteDbSetup.Init()
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Approved, UserId.ToString())
                    .WithTonnages(categories1).WithRecipient(scheme.Id).Create());

                // note to not be included no matching category
                var categories2 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 2, null)
                };
                notesSetToBeIncluded.Add(EvidenceNoteDbSetup.Init()
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Approved, UserId.ToString())
                    .WithTonnages(categories2).WithRecipient(scheme.Id).Create());

                // note to not be included not in note list
                var categories3 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 3, null)
                };
                notesSetToNotBeIncluded.Add(EvidenceNoteDbSetup.Init()
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Approved, UserId.ToString())
                    .WithTonnages(categories3).WithRecipient(scheme.Id).Create());

                request = new GetEvidenceNotesForTransferRequest(organisation.Id,
                    new List<int>()
                    {
                        Core.DataReturns.WeeeCategory.AutomaticDispensers.ToInt()
                    }, new List<Guid>()
                    {
                        notesSetToBeIncluded.ElementAt(0).Id,
                        notesSetToBeIncluded.ElementAt(1).Id
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

                var note1 = result.First(r => r.Id.Equals(notesSetToBeIncluded.ElementAt(0).Id));
                note1.EvidenceTonnageData.Count.Should().Be(1);
                note1.EvidenceTonnageData.ElementAt(0).Received.Should().Be(1);
                note1.EvidenceTonnageData.ElementAt(0).Reused.Should().Be(null);
                note1.EvidenceTonnageData.ElementAt(0).Id.Should().NotBe(Guid.Empty);
                note1.EvidenceTonnageData.ElementAt(0).CategoryId.Should()
                    .Be(Core.DataReturns.WeeeCategory.AutomaticDispensers);

                var note2 = result.First(r => r.Id.Equals(notesSetToBeIncluded.ElementAt(1).Id));
                note2.EvidenceTonnageData.Count.Should().Be(1);
                note2.EvidenceTonnageData.ElementAt(0).Received.Should().Be(2);
                note2.EvidenceTonnageData.ElementAt(0).Reused.Should().Be(null);
                note2.EvidenceTonnageData.ElementAt(0).Id.Should().NotBe(Guid.Empty);
                note2.EvidenceTonnageData.ElementAt(0).CategoryId.Should()
                    .Be(Core.DataReturns.WeeeCategory.AutomaticDispensers);

                var notIncluded = result.FirstOrDefault(r => r.Id.Equals(notesSetToNotBeIncluded.ElementAt(0).Id));
                notIncluded.Should().BeNull();
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
            protected static IRequestHandler<GetEvidenceNotesForTransferRequest, IList<EvidenceNoteData>> handler;
            protected static Organisation organisation;
            protected static GetEvidenceNotesForTransferRequest request;
            protected static IList<EvidenceNoteData> result;
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
                handler = Container.Resolve<IRequestHandler<GetEvidenceNotesForTransferRequest, IList<EvidenceNoteData>>>();

                return setup;
            }
        }
    }
}