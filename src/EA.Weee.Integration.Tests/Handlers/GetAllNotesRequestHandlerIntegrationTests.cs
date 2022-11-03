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
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain.Evidence;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Requests.Admin;
    using FluentAssertions;
    using NUnit.Specifications;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using NoteStatus = Core.AatfEvidence.NoteStatus;
    using NoteStatusDomain = Domain.Evidence.NoteStatus;
    using NoteType = Core.AatfEvidence.NoteType;
    using WasteType = Core.AatfEvidence.WasteType;

    public class GetAllNotesRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIGetAListOfEvidenceNoteDataAsNotesWithCorrectStatusAndType : GetAllNotesRequestHandlerTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var complianceYear = fixture.Create<int>();

                var evidenceWithApprovedStatus = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                        n.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString(), SystemTime.UtcNow);
                    })
                    .WithComplianceYear(complianceYear)
                    .Create();

                var evidenceWithSubmittedStatus = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                    })
                    .WithComplianceYear(complianceYear)
                    .Create();

                var evidenceWithReturnedStatus = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Returned, UserId.ToString(), SystemTime.UtcNow);
                    })
                    .WithComplianceYear(complianceYear)
                    .Create();

                var evidenceWithRejectedStatus = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Rejected, UserId.ToString(), SystemTime.UtcNow);
                    })
                    .WithComplianceYear(complianceYear)
                    .Create();

                var evidenceWithVoidStatus = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                        n.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString(), SystemTime.UtcNow);
                        n.UpdateStatus(NoteStatusDomain.Void, UserId.ToString(), SystemTime.UtcNow);
                    })
                    .WithComplianceYear(complianceYear)
                    .Create();

                notesSet.Add(evidenceWithApprovedStatus);
                notesSet.Add(evidenceWithSubmittedStatus);
                notesSet.Add(evidenceWithReturnedStatus);
                notesSet.Add(evidenceWithRejectedStatus);
                notesSet.Add(evidenceWithVoidStatus);

                request = new GetAllNotesInternal(noteTypeFilter, allowedStatuses, complianceYear, 1, int.MaxValue,
                    null, null, null, null, null, null, null, null);
            };

            private readonly Because of = () =>
            {
                evidenceNoteData = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldReturnListOfEvidenceNotes = () =>
            {
                evidenceNoteData.Should().NotBeNull();
            };

            private readonly It shouldHaveExpectedResultsCountToBeSetOfNotes = () =>
            {
                evidenceNoteData.Results.Should().HaveCount(notesSet.Count);
                evidenceNoteData.NoteCount.Should().Be(notesSet.Count);
            };

            private readonly It shouldHaveExpectedData = () =>
            {
                foreach (var note1 in notesSet)
                {
                    var evidenceNote = evidenceNoteData.Results.ToList().Exists(n => n.Id == note1.Id);
                    evidenceNote.Should().BeTrue();
                }
            };
        }

        [Component]
        public class WhenIGetAListOfEvidenceNoteDataAsNotesWithNotAllowedStatusAndType : GetAllNotesRequestHandlerTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var evidenceWithDraftStatus1 = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .Create();

                var evidenceWithDraftStatus2 = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .Create();

                notesSet.Add(evidenceWithDraftStatus1);
                notesSet.Add(evidenceWithDraftStatus2);

                request = new GetAllNotesInternal(noteTypeFilterForTransferNote, notAllowedStatuses, SystemTime.UtcNow.Year, 1, int.MaxValue,
                    null, null, null, null, null, null, null, null);
            };

            private readonly Because of = () =>
            {
                evidenceNoteData = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldReturnEmptyListOfEvidenceNotes = () =>
            {
                evidenceNoteData.Results.Should().BeNullOrEmpty();
            };

            private readonly It shouldHaveExpectedResultsCountToBeSetOfNotes = () =>
            {
                evidenceNoteData.Results.Should().HaveCount(0);
                evidenceNoteData.NoteCount.Should().Be(0);
            };
        }

        [Component]
        public class WhenIGetAListOfEvidenceNoteDataAsNotesWithStartAndEndDateSubmittedFilter : GetAllNotesRequestHandlerTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var complianceYear = fixture.Create<int>();

                var evidence1WithinSubmittedDates = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow.AddDays(9));
                        n.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString(), SystemTime.UtcNow);
                    })
                    .WithComplianceYear(complianceYear)
                    .Create();

                var evidence2WithinSubmittedDates = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                     n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow.AddDays(-9));
                    })
                    .WithComplianceYear(complianceYear)
                    .Create();

                var evidence3WithinSubmittedDates = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                    })
                    .WithComplianceYear(complianceYear)
                    .Create();

                var evidence4WithinSubmittedDates = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow.AddDays(3));
                    })
                    .WithComplianceYear(complianceYear)
                    .Create();

                var evidence1NotInSubmittedDates = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow.AddDays(11));
                    })
                    .WithComplianceYear(complianceYear)
                    .Create();

                var evidence2NotInSubmittedDates = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow.AddDays(-11));
                    })
                    .WithComplianceYear(complianceYear)
                    .Create();

                notesSet.Add(evidence1WithinSubmittedDates);
                notesSet.Add(evidence2WithinSubmittedDates);
                notesSet.Add(evidence3WithinSubmittedDates);
                notesSet.Add(evidence4WithinSubmittedDates);
                notesSet.Add(evidence1NotInSubmittedDates);
                notesSet.Add(evidence2NotInSubmittedDates);

                request = new GetAllNotesInternal(noteTypeFilter, allowedStatuses, complianceYear, 1, int.MaxValue,
                    startDateSubmittedFilter, endDateSubmittedFilter, null, null, null, null, null, null);
            };

            private readonly Because of = () =>
            {
                evidenceNoteData = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldReturnListOfEvidenceNotes = () =>
            {
                evidenceNoteData.Should().NotBeNull();
            };

            private readonly It shouldHaveExpectedResultsCountToBeSetOfNotes = () =>
            {
                evidenceNoteData.Results.Should().HaveCount(4);
                evidenceNoteData.NoteCount.Should().Be(4);
            };

            private readonly It shouldHaveExpectedData = () =>
            {
                foreach (var note1 in notesSet.Take(4))
                {
                    var evidenceNote = evidenceNoteData.Results.ToList().Exists(n => n.Id == note1.Id);
                    evidenceNote.Should().BeTrue();
                }
            };
        }

        [Component]
        public class WhenIGetAListOfEvidenceNoteDataAsNotesWithStartDateSubmittedFilterAndEndSubmittedDateIsNull : GetAllNotesRequestHandlerTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var complianceYear = fixture.Create<int>();

                var evidence1WithinSubmittedDates = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow.AddDays(-10));
                        n.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString(), SystemTime.UtcNow);
                    })
                    .WithComplianceYear(complianceYear)
                    .Create();

                var evidence2WithinSubmittedDates = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow.AddDays(-9));
                    })
                    .WithComplianceYear(complianceYear)
                    .Create();

                var evidence3WithinSubmittedDates = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                    })
                    .WithComplianceYear(complianceYear)
                    .Create();

                var evidence1NotInSubmittedDates = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow.AddDays(-11));
                    })
                    .WithComplianceYear(complianceYear)
                    .Create();

                var evidence2NotInSubmittedDates = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow.AddDays(-40));
                    })
                    .WithComplianceYear(complianceYear)
                    .Create();

                notesSet.Add(evidence1WithinSubmittedDates);
                notesSet.Add(evidence2WithinSubmittedDates);
                notesSet.Add(evidence3WithinSubmittedDates);
                notesSet.Add(evidence1NotInSubmittedDates);
                notesSet.Add(evidence2NotInSubmittedDates);

                request = new GetAllNotesInternal(noteTypeFilter, allowedStatuses, complianceYear, 1, int.MaxValue,
                    startDateSubmittedFilter, null, null, null, null, null, null, null);
            };

            private readonly Because of = () =>
            {
                evidenceNoteData = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldReturnListOfEvidenceNotes = () =>
            {
                evidenceNoteData.Should().NotBeNull();
            };

            private readonly It shouldHaveExpectedResultsCountToBeSetOfNotes = () =>
            {
                evidenceNoteData.Results.Should().HaveCount(3);
                evidenceNoteData.NoteCount.Should().Be(3);
            };

            private readonly It shouldHaveExpectedData = () =>
            {
                foreach (var note1 in notesSet.Take(3))
                {
                    var evidenceNote = evidenceNoteData.Results.ToList().Exists(n => n.Id == note1.Id);
                    evidenceNote.Should().BeTrue();
                }
            };
        }

        [Component]
        public class WhenIGetAListOfEvidenceNoteDataAsNotesWithEndDateSubmittedFilterAndStartSubmittedDateIsNull : GetAllNotesRequestHandlerTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var complianceYear = fixture.Create<int>();

                var evidence1WithinSubmittedDates = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow.AddDays(10));
                        n.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString(), SystemTime.UtcNow);
                    })
                    .WithComplianceYear(complianceYear)
                    .Create();

                var evidence2WithinSubmittedDates = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                    })
                    .WithComplianceYear(complianceYear)
                    .Create();

                var evidence1NotInSubmittedDates = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow.AddDays(14));
                    })
                    .WithComplianceYear(complianceYear)
                    .Create();

                var evidence2NotInSubmittedDates = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow.AddDays(40));
                    })
                    .WithComplianceYear(complianceYear)
                    .Create();

                notesSet.Add(evidence1WithinSubmittedDates);
                notesSet.Add(evidence2WithinSubmittedDates);
                notesSet.Add(evidence1NotInSubmittedDates);
                notesSet.Add(evidence2NotInSubmittedDates);

                request = new GetAllNotesInternal(noteTypeFilter, allowedStatuses, complianceYear, 1, int.MaxValue,
                    null, endDateSubmittedFilter, null, null, null, null, null, null);
            };

            private readonly Because of = () =>
            {
                evidenceNoteData = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldReturnListOfEvidenceNotes = () =>
            {
                evidenceNoteData.Should().NotBeNull();
            };

            private readonly It shouldHaveExpectedResultsCountToBeSetOfNotes = () =>
            {
                evidenceNoteData.Results.Should().HaveCount(2);
                evidenceNoteData.NoteCount.Should().Be(2);
            };

            private readonly It shouldHaveExpectedData = () =>
            {
                foreach (var note1 in notesSet.Take(2))
                {
                    var evidenceNote = evidenceNoteData.Results.ToList().Exists(n => n.Id == note1.Id);
                    evidenceNote.Should().BeTrue();
                }
            };
        }

        [Component]
        public class WhenIGetAListOfEvidenceNoteDataAsNotesWithRecipientIdInFilter : GetAllNotesRequestHandlerTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var complianceYear = fixture.Create<int>();

                organisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(organisation.Id).Create();

                recipientOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation.Id).Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, recipientOrganisation.Id).Create();

                var evidence1WithRecipient = EvidenceNoteDbSetup.Init()
                 .With(n =>
                 {
                     n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                     n.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString(), SystemTime.UtcNow);
                 })
                 .WithComplianceYear(complianceYear)
                 .WithRecipient(recipientOrganisation.Id)
                 .Create();

                var evidence2WithRecipient = EvidenceNoteDbSetup.Init()
                 .With(n =>
                 {
                     n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                 })
                 .WithComplianceYear(complianceYear)
                 .WithRecipient(recipientOrganisation.Id)
                 .Create();

                var evidence1NoRecipient = EvidenceNoteDbSetup.Init()
                .With(n =>
                {
                    n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow.AddDays(14));
                })
                .WithComplianceYear(complianceYear)
                .WithRecipient(organisation.Id)
                .Create();

                var evidence2NoRecipient = EvidenceNoteDbSetup.Init()
                 .With(n =>
                 {
                     n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow.AddDays(40));
                 })
                 .WithComplianceYear(complianceYear)
                 .WithRecipient(organisation.Id)
                 .Create();

                notesSet.Add(evidence1WithRecipient);
                notesSet.Add(evidence2WithRecipient);
                notesSet.Add(evidence1NoRecipient);
                notesSet.Add(evidence2NoRecipient);

                request = new GetAllNotesInternal(noteTypeFilter, allowedStatuses, complianceYear, 1, int.MaxValue,
                    null, null, recipientOrganisation.Id, null, null, null, null, null);
            };

            private readonly Because of = () =>
            {
                evidenceNoteData = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldReturnListOfEvidenceNotes = () =>
            {
                evidenceNoteData.Should().NotBeNull();
            };

            private readonly It shouldHaveExpectedResultsCountToBeSetOfNotes = () =>
            {
                evidenceNoteData.Results.Should().HaveCount(2);
                evidenceNoteData.NoteCount.Should().Be(2);
            };

            private readonly It shouldHaveExpectedData = () =>
            {
                foreach (var note1 in notesSet.Take(2))
                {
                    var evidenceNote = evidenceNoteData.Results.ToList().Exists(n => n.Id == note1.Id);
                    evidenceNote.Should().BeTrue();
                }
            };
        }

        [Component]
        public class WhenIGetAListOfEvidenceNoteDataAsNotesWithNoteStatusSetInFilter : GetAllNotesRequestHandlerTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var complianceYear = fixture.Create<int>();

                var evidence1NotVoidStatus = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                        n.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString(), SystemTime.UtcNow.AddDays(10));
                    })
                    .WithComplianceYear(complianceYear)
                    .Create();

                var evidence2NotVoidStatus = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow.AddDays(-3));
                        n.UpdateStatus(NoteStatusDomain.Returned, UserId.ToString(), SystemTime.UtcNow);
                    })
                    .WithComplianceYear(complianceYear)
                    .Create();

                var evidence3NotVoidStatus = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                        n.UpdateStatus(NoteStatusDomain.Draft, UserId.ToString(), SystemTime.UtcNow.AddDays(2));
                    })
                    .WithComplianceYear(complianceYear)
                    .Create();

                var evidence4NotVoidStatus = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow.AddDays(-1));
                        n.UpdateStatus(NoteStatusDomain.Rejected, UserId.ToString(), SystemTime.UtcNow.AddDays(1));
                    })
                    .WithComplianceYear(complianceYear)
                    .Create();

                var evidence1WithVoidStatus = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                        n.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString(), SystemTime.UtcNow);
                        n.UpdateStatus(NoteStatusDomain.Void, UserId.ToString(), SystemTime.UtcNow);
                    })
                    .WithComplianceYear(complianceYear)
                    .Create();

                var evidence2WithVoidStatus = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                        n.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString(), SystemTime.UtcNow.AddDays(-1));
                        n.UpdateStatus(NoteStatusDomain.Void, UserId.ToString(), SystemTime.UtcNow.AddDays(4));
                    })
                    .WithComplianceYear(complianceYear)
                    .Create();

                notesSet.Add(evidence1NotVoidStatus);
                notesSet.Add(evidence2NotVoidStatus);
                notesSet.Add(evidence3NotVoidStatus);
                notesSet.Add(evidence4NotVoidStatus);
                notesSet.Add(evidence1WithVoidStatus);
                notesSet.Add(evidence2WithVoidStatus);

                request = new GetAllNotesInternal(noteTypeFilter, allowedStatuses, complianceYear, 1, int.MaxValue,
                    null, null, null, NoteStatus.Void, null, null, null, null);
            };

            private readonly Because of = () =>
            {
                evidenceNoteData = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldReturnListOfEvidenceNotes = () =>
            {
                evidenceNoteData.Should().NotBeNull();
            };

            private readonly It shouldHaveExpectedResultsCountToBeSetOfNotes = () =>
            {
                evidenceNoteData.Results.Should().HaveCount(2);
                evidenceNoteData.NoteCount.Should().Be(2);
            };

            private readonly It shouldHaveExpectedData = () =>
            {
                foreach (var note1 in notesSet.Skip(4))
                {
                    var evidenceNote = evidenceNoteData.Results.ToList().Exists(n => n.Id == note1.Id);
                    evidenceNote.Should().BeTrue();
                }
            };
        }

        [Component]
        public class WhenIGetAListOfEvidenceNoteDataAsNotesWithObligationTypeSetInFilter : GetAllNotesRequestHandlerTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var complianceYear = fixture.Create<int>();

                var evidence1HouseholdType = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                        n.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString(), SystemTime.UtcNow.AddDays(10));
                    })
                    .WithComplianceYear(complianceYear)
                    .WithWasteType(Domain.Evidence.WasteType.HouseHold)
                    .Create();

                var evidence2HouseholdType = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                        n.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString(), SystemTime.UtcNow.AddDays(10));
                    })
                    .WithComplianceYear(complianceYear)
                    .WithWasteType(Domain.Evidence.WasteType.HouseHold)
                    .Create();

                var evidence1NonHouseholdType = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                        n.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString(), SystemTime.UtcNow.AddDays(10));
                    })
                    .WithComplianceYear(complianceYear)
                    .WithWasteType(Domain.Evidence.WasteType.NonHouseHold)
                    .Create();

                notesSet.Add(evidence1HouseholdType);
                notesSet.Add(evidence2HouseholdType);
                notesSet.Add(evidence1NonHouseholdType);

                request = new GetAllNotesInternal(noteTypeFilter, allowedStatuses, complianceYear, 1, int.MaxValue,
                    null, null, null, null, WasteType.Household, null, null, null);
            };

            private readonly Because of = () =>
            {
                evidenceNoteData = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldReturnListOfEvidenceNotes = () =>
            {
                evidenceNoteData.Should().NotBeNull();
            };

            private readonly It shouldHaveExpectedResultsCountToBeSetOfNotes = () =>
            {
                evidenceNoteData.Results.Should().HaveCount(2);
                evidenceNoteData.NoteCount.Should().Be(2);
            };

            private readonly It shouldHaveExpectedData = () =>
            {
                foreach (var note1 in notesSet.Take(2))
                {
                    var evidenceNote = evidenceNoteData.Results.ToList().Exists(n => n.Id == note1.Id);
                    evidenceNote.Should().BeTrue();
                }
            };
        }

        [Component]
        public class WhenIGetAListOfEvidenceNoteDataAsNotesWithSubmittedByAatfIdSetInFilter : GetAllNotesRequestHandlerTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();
                var organisationWithSelectedAatf = OrganisationDbSetup.Init().Create();

                var organisationWithNonSelectedAatf = OrganisationDbSetup.Init().Create();

                var aatfSelected = AatfDbSetup.Init()
                    .WithAppropriateAuthority(CompetentAuthority.Scotland)
                    .WithOrganisation(organisationWithSelectedAatf.Id)
                    .Create();

                var aatfNonSelected = AatfDbSetup.Init()
                    .WithAppropriateAuthority(CompetentAuthority.Wales)
                    .WithOrganisation(organisationWithNonSelectedAatf.Id)
                    .Create();

                var complianceYear = fixture.Create<int>();

                var evidence1WithSelectedAatf = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                    })
                    .WithComplianceYear(complianceYear)
                    .WithAatf(aatfSelected.Id)
                    .Create();

                var evidence2WithSelectedAatf = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow.AddDays(-1));
                        n.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString(), SystemTime.UtcNow);
                    })
                    .WithComplianceYear(complianceYear)
                    .WithAatf(aatfSelected.Id)
                    .Create();

                var evidence3WithSelectedAatf = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Returned, UserId.ToString(), SystemTime.UtcNow);
                    })
                    .WithComplianceYear(complianceYear)
                    .WithAatf(aatfSelected.Id)
                    .Create();

                var evidence1WithNonSelectedAatf = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                        n.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString(), SystemTime.UtcNow);
                    })
                    .WithComplianceYear(complianceYear)
                    .WithAatf(aatfNonSelected.Id)
                    .Create();

                var evidence2WithNonSelectedAatf = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                    })
                    .WithComplianceYear(complianceYear)
                    .WithAatf(aatfNonSelected.Id)
                    .Create();

                notesSet.Add(evidence1WithSelectedAatf);
                notesSet.Add(evidence2WithSelectedAatf);
                notesSet.Add(evidence3WithSelectedAatf);
                notesSet.Add(evidence1WithNonSelectedAatf);
                notesSet.Add(evidence2WithNonSelectedAatf);

                request = new GetAllNotesInternal(noteTypeFilter, allowedStatuses, complianceYear, 1, int.MaxValue,
                    null, null, null, null, null, aatfSelected.Id, null, null);
            };

            private readonly Because of = () =>
            {
                evidenceNoteData = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldReturnListOfEvidenceNotes = () =>
            {
                evidenceNoteData.Should().NotBeNull();
            };

            private readonly It shouldHaveExpectedResultsCountToBeSetOfNotes = () =>
            {
                evidenceNoteData.Results.Should().HaveCount(3);
                evidenceNoteData.NoteCount.Should().Be(3);
            };

            private readonly It shouldHaveExpectedData = () =>
            {
                foreach (var note1 in notesSet.Take(3))
                {
                    var evidenceNote = evidenceNoteData.Results.ToList().Exists(n => n.Id == note1.Id);
                    evidenceNote.Should().BeTrue();
                }
            };
        }

        [Component]
        public class WhenIGetAListOfEvidenceNoteDataAsNotesWithSearchRefSetInFilter : GetAllNotesRequestHandlerTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(organisation.Id).Create();

                recipientOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation.Id).Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, recipientOrganisation.Id).Create();

                var complianceYear = fixture.Create<int>();

                var evidenceWithFilteredReference = EvidenceNoteDbSetup.Init()
                    .WithComplianceYear(complianceYear)
                    .WithRecipient(recipientOrganisation.Id)
                    .Create();

                var evidence1WithNotSelectedReference = EvidenceNoteDbSetup.Init()
                    .WithRecipient(recipientOrganisation.Id)
                    .WithComplianceYear(complianceYear)
                    .Create();

                var evidence2WithNotSelectedReference = EvidenceNoteDbSetup.Init()
                    .WithRecipient(recipientOrganisation.Id)
                    .WithComplianceYear(complianceYear)
                    .Create();

                notesSet.Add(evidenceWithFilteredReference);
                notesSet.Add(evidence1WithNotSelectedReference);
                notesSet.Add(evidence2WithNotSelectedReference);

                filterReference = evidenceWithFilteredReference.Reference;

                request = new GetAllNotesInternal(noteTypeFilter, new List<NoteStatus> { NoteStatus.Draft }, complianceYear, 1, int.MaxValue,
                    null, null, null, null, null, null, filterReference.ToString(), null);
            };

            private readonly Because of = () =>
            {
                evidenceNoteData = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldReturnListOfEvidenceNotes = () =>
            {
                evidenceNoteData.Should().NotBeNull();
            };

            private readonly It shouldHaveExpectedResultsCountToBeSetOfNotes = () =>
            {
                evidenceNoteData.Results.Should().HaveCount(1);
                evidenceNoteData.NoteCount.Should().Be(1);
            };

            private readonly It shouldHaveExpectedData = () =>
            {
                foreach (var note1 in notesSet.Take(1))
                {
                    var evidenceNote = evidenceNoteData.Results.ToList().Exists(n => n.Id == note1.Id);
                    evidenceNote.Should().BeTrue();
                    note1.Reference.Should().Be(filterReference);
                }
            };
        }

        public class GetAllNotesRequestHandlerTestBase : WeeeContextSpecification
        {
            protected static EvidenceNoteSearchDataResult evidenceNoteData;
            protected static Organisation organisation;
            protected static Organisation recipientOrganisation;
            protected static List<Note> notesSet;
            protected static List<NoteStatus> allowedStatuses;
            protected static List<NoteStatus> notAllowedStatuses;
            protected static List<NoteType> noteTypeFilter;
            protected static List<NoteType> noteTypeFilterForTransferNote;
            protected static IRequestHandler<GetAllNotesInternal, EvidenceNoteSearchDataResult> handler;
            protected static Fixture fixture;
            protected static GetAllNotesInternal request;
            protected static DateTime? startDateSubmittedFilter;
            protected static DateTime? endDateSubmittedFilter;
            protected static NoteStatus? noteStatusFilter;
            protected static WasteType? obligationTypeFilter;
            protected static Guid? submittedAatfIdFilter;
            protected static int filterReference;

            public static void LocalSetup()
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings(resetDb: true)
                    .WithInternalUserAccess();

                var authority = Query.GetEaCompetentAuthority();
                var role = Query.GetInternalUserRole();

                if (!Query.CompetentAuthorityUserExists(UserId.ToString(), role.Id))
                {
                    CompetentAuthorityUserDbSetup.Init().WithUserIdAndAuthorityAndRole(UserId.ToString(), authority.Id, role.Id)
                        .Create();
                }

                fixture = new Fixture();
                notesSet = new List<Note>();
                allowedStatuses = new List<NoteStatus> { NoteStatus.Approved, NoteStatus.Rejected, NoteStatus.Submitted, NoteStatus.Void, NoteStatus.Returned };
                notAllowedStatuses = new List<NoteStatus> { NoteStatus.Draft };
                noteTypeFilter = new List<NoteType> { NoteType.Evidence };
                noteTypeFilterForTransferNote = new List<NoteType> { NoteType.Transfer };
                handler = Container.Resolve<IRequestHandler<GetAllNotesInternal, EvidenceNoteSearchDataResult>>();
                startDateSubmittedFilter = DateTime.Now.AddDays(-10);
                endDateSubmittedFilter = DateTime.Now.AddDays(10);
                noteStatusFilter = NoteStatus.Approved;
                obligationTypeFilter = WasteType.Household;
                submittedAatfIdFilter = fixture.Create<Guid>();
            }
        }
    }
}
