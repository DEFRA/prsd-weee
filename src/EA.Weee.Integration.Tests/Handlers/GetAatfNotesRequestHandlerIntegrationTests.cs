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
    using Domain.Organisation;
    using Domain.Scheme;
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.Evidence;
    using EA.Weee.Domain.User;
    using EA.Weee.Requests.AatfEvidence;
    using FluentAssertions;
    using NUnit.Specifications;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using NoteStatus = Core.AatfEvidence.NoteStatus;
    using NoteStatusDomain = Domain.Evidence.NoteStatus;

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
                CatchExceptionAsync(() => handler.HandleAsync(new GetAatfNotesRequest(Guid.NewGuid(), aatf.Id, new List<Core.AatfEvidence.NoteStatus> { Core.AatfEvidence.NoteStatus.Draft }, null, SystemTime.UtcNow.Year, null, null, null, null, null)));
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
                .WithOrganisation(organisation.Id)
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
                evidenceNoteData = Task.Run(async () => await handler.HandleAsync(new GetAatfNotesRequest(organisation.Id, aatf.Id, new List<NoteStatus> { NoteStatus.Draft }, null, SystemTime.UtcNow.Year, null, null, null, null, null))).Result;
            };

            private readonly It shouldReturnListOfEvidenceNotes = () =>
            {
                evidenceNoteData.Should().NotBeNull();
            };

            private readonly It shouldHaveExpectedResultsCountToBeSetOfNotes = () =>
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
                evidenceNoteData.Select(e => e.Reference).Should().BeInDescendingOrder();
            };
        }

        [Component]
        public class WhenNoEvidenceShouldBeReturnAsTheAllowedStatusesDoNotPermitIt : GetAatfNotesRequestHandlerTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init()
               .Create();

                aatf = AatfDbSetup.Init()
                .WithOrganisation(organisation.Id)
                .Create();

                OrganisationUserDbSetup.Init()
                .WithUserIdAndOrganisationId(UserId, organisation.Id)
                .WithStatus(UserStatus.Active)
                .Create();

                var evidence1 = EvidenceNoteDbSetup.Init()
                .WithOrganisation(organisation.Id)
                .WithAatf(aatf.Id)
                .WithStatus(NoteStatusDomain.Void, UserId.ToString())
                .Create();

                var evidence2 = EvidenceNoteDbSetup.Init()
                .WithOrganisation(organisation.Id)
                .WithAatf(aatf.Id)
                .WithStatus(NoteStatusDomain.Rejected, UserId.ToString())
                .Create();

                var evidence3 = EvidenceNoteDbSetup.Init()
                .WithOrganisation(organisation.Id)
                .WithAatf(aatf.Id)
                .WithStatus(NoteStatusDomain.Rejected, UserId.ToString())
                .Create();

                var evidence4 = EvidenceNoteDbSetup.Init()
                .WithOrganisation(organisation.Id)
                .WithAatf(aatf.Id)
                .WithStatus(NoteStatusDomain.Void, UserId.ToString())
                .Create();
            };

            private readonly Because of = () =>
            {
                evidenceNoteData = Task.Run(async () => await handler.HandleAsync(new GetAatfNotesRequest(organisation.Id, aatf.Id, new List<NoteStatus> { NoteStatus.Draft }, null, SystemTime.UtcNow.Year, null, null, null, null, null))).Result;
            };

            private readonly It shouldReturnNoData = () =>
            {
                evidenceNoteData.Should().BeNullOrEmpty();
            };
        }

        [Component]
        public class WhenIGetFilteredEvidencesBasedOnAllowedStatusesList : GetAatfNotesRequestHandlerTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init()
               .Create();

                aatf = AatfDbSetup.Init()
                .WithOrganisation(organisation.Id)
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
                .WithStatus(NoteStatusDomain.Submitted, UserId.ToString())
                .Create();

                var evidence3 = EvidenceNoteDbSetup.Init()
                .WithOrganisation(organisation.Id)
                .WithAatf(aatf.Id)
                .With(n =>
                {
                    n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString());
                    n.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString());
                })
                .Create();

                var evidence4 = EvidenceNoteDbSetup.Init()
                .WithOrganisation(organisation.Id)
                .WithAatf(aatf.Id)
                .With(n =>
                {
                    n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString());
                    n.UpdateStatus(NoteStatusDomain.Rejected, UserId.ToString());
                })
                .Create();

                var evidence5 = EvidenceNoteDbSetup.Init()
                .WithOrganisation(organisation.Id)
                .WithAatf(aatf.Id)
                .With(n =>
                {
                    n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString());
                    n.UpdateStatus(NoteStatusDomain.Void, UserId.ToString());
                })
                .Create();
            };

            private readonly Because of = () =>
            {
                evidenceNoteData = Task.Run(async () => await handler.HandleAsync(new GetAatfNotesRequest(organisation.Id, aatf.Id, allowedStatuses, null, SystemTime.UtcNow.Year, null, null, null, null, null))).Result;
            };

            private readonly It shouldReturnAFilteredListBasedOnAllowedStatuses = () =>
            {
                evidenceNoteData.Should().NotBeNull();
            };
            
            private readonly It shouldHaveExpectedResultsCount = () =>
            {
                evidenceNoteData.Should().HaveCount(allowedStatuses.Count);
            };

            private readonly It shouldHaveExpectedAllowedStatuses = () =>
            {
                var allowedStatusesToArray = allowedStatuses.ToArray();

                foreach (var note in evidenceNoteData)
                {
                    note.Status.Should().BeOneOf(allowedStatusesToArray);  
                }
            };
        }

        [Component]
        public class WhenIGetApprovedEvidencesBasedOnComplianceYear : GetAatfNotesRequestHandlerTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init()
               .Create();

                aatf = AatfDbSetup.Init()
                .WithOrganisation(organisation.Id)
                .Create();

                OrganisationUserDbSetup.Init()
                .WithUserIdAndOrganisationId(UserId, organisation.Id)
                .WithStatus(UserStatus.Active)
                .Create();                

                var evidence1 = EvidenceNoteDbSetup.Init()
                .WithOrganisation(organisation.Id)
                .WithAatf(aatf.Id)
                .With(n =>
                {
                    n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString());
                    n.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString());
                })
                .WithComplianceYear(complianceYear)
                .Create();

                var evidence2 = EvidenceNoteDbSetup.Init()
                .WithOrganisation(organisation.Id)
                .WithAatf(aatf.Id)
                .With(n =>
                {
                    n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString());
                    n.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString());
                })
                .WithComplianceYear(complianceYear)
                .Create();

                var evidence3 = EvidenceNoteDbSetup.Init()
                .WithOrganisation(organisation.Id)
                .WithAatf(aatf.Id)
                .With(n =>
                {
                    n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString());
                    n.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString());
                })
                .WithComplianceYear(complianceYear)
                .Create();

                var evidence4 = EvidenceNoteDbSetup.Init()
                .WithOrganisation(organisation.Id)
                .WithAatf(aatf.Id)
                .With(n =>
                {
                    n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString());
                    n.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString());
                })
                .WithComplianceYear(complianceYear)
                .Create();

                var evidence5 = EvidenceNoteDbSetup.Init()
                .WithOrganisation(organisation.Id)
                .WithAatf(aatf.Id)
                .With(n =>
                {
                    n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString());
                    n.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString());
                })
                .WithComplianceYear(complianceYear - 1)
                .Create();

                var evidence6 = EvidenceNoteDbSetup.Init()
                .WithOrganisation(organisation.Id)
                .WithAatf(aatf.Id)
                .With(n =>
                {
                    n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString());
                    n.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString());
                })
                .WithComplianceYear(complianceYear - 1)
                .Create();
            };

            private readonly Because of = () =>
            {
                evidenceNoteData = Task.Run(async () => await handler.HandleAsync(new GetAatfNotesRequest(organisation.Id, aatf.Id, allowedStatuses, null, complianceYear, null, null, null, null, null))).Result;
            };

            private readonly It shouldReturnAFilteredListBasedOnAllowedStatuses = () =>
            {
                evidenceNoteData.Should().NotBeNull();
                evidenceNoteData.Count.Should().Be(4);
            };

            private readonly It shouldHaveExpectedResultsCount = () =>
            {
                evidenceNoteData.Count(n => n.ComplianceYear == complianceYear).Should().Be(4);
                evidenceNoteData.Count(n => n.ComplianceYear == SystemTime.UtcNow.AddYears(-1).Year).Should().Be(0);
            };

            private readonly It shouldHaveExpectedAllowedStatuses = () =>
            {
                var allowedStatusesToArray = allowedStatuses.ToArray();

                foreach (var evidenceNote in evidenceNoteData)
                {
                    evidenceNote.Status.Should().BeOneOf(allowedStatusesToArray);
                }
            };
        }

        [Component]
        public class WhenIGetEvidenceNotesBasedSameComplianceYear : GetAatfNotesRequestHandlerTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init()
               .Create();

                aatf = AatfDbSetup.Init()
                .WithOrganisation(organisation.Id)
                .Create();

                OrganisationUserDbSetup.Init()
                .WithUserIdAndOrganisationId(UserId, organisation.Id)
                .WithStatus(UserStatus.Active)
                .Create();

                var evidence1 = EvidenceNoteDbSetup.Init()
                .WithOrganisation(organisation.Id)
                .WithAatf(aatf.Id)
                .With(n =>
                {
                    n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString());
                    n.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString());
                })
                .WithComplianceYear(complianceYear - 1)
                .Create();

                var evidence2 = EvidenceNoteDbSetup.Init()
                .WithOrganisation(organisation.Id)
                .WithAatf(aatf.Id)
                .With(n =>
                {
                    n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString());
                    n.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString());
                })
                .WithComplianceYear(complianceYear)
                .Create();

                var evidence3 = EvidenceNoteDbSetup.Init()
                .WithOrganisation(organisation.Id)
                .WithAatf(aatf.Id)
                .With(n =>
                {
                    n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString());
                    n.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString());
                })
                .WithComplianceYear(complianceYear)
                .Create();

                var evidence4 = EvidenceNoteDbSetup.Init()
                .WithOrganisation(organisation.Id)
                .WithAatf(aatf.Id)
                .With(n =>
                {
                    n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString());
                    n.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString());
                })
                .WithComplianceYear(complianceYear + 1)
                .Create();
            };

            private readonly Because of = () =>
            {
                evidenceNoteData = Task.Run(async () => await handler.HandleAsync(new GetAatfNotesRequest(organisation.Id, aatf.Id, allowedStatuses, null, complianceYear, null, null, null, null, null))).Result;
            };

            private readonly It shouldHaveExpectedResultsCount = () =>
            {
                evidenceNoteData.Count(n => n.ComplianceYear == complianceYear).Should().Be(2);
                evidenceNoteData.Count(n => n.ComplianceYear == complianceYear - 1).Should().Be(0);
                evidenceNoteData.Count(n => n.ComplianceYear == complianceYear + 1).Should().Be(0);
            };
        }

        public class GetAatfNotesRequestHandlerTestBase : WeeeContextSpecification
        {
            protected static List<EvidenceNoteData> evidenceNoteData;
            protected static List<Note> notesSet;
            protected static List<NoteStatus> allowedStatuses;
            protected static IRequestHandler<GetAatfNotesRequest, List<EvidenceNoteData>> handler;
            protected static Organisation organisation;
            protected static Scheme scheme;
            protected static Aatf aatf;
            protected static Note note;
            protected static Fixture fixture;
            protected static int complianceYear;

            public static void LocalSetup()
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings(resetDb: true)
                    .WithExternalUserAccess();

                fixture = new Fixture();
                complianceYear = fixture.Create<int>();
                notesSet = new List<Note>();
                allowedStatuses = new List<NoteStatus> { NoteStatus.Approved };
                handler = Container.Resolve<IRequestHandler<GetAatfNotesRequest, List<EvidenceNoteData>>>();
            }
        }
    }
}
