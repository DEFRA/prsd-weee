namespace EA.Weee.Integration.Tests.Handlers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Autofac;
    using AutoFixture;
    using Base;
    using Builders;
    using EA.Prsd.Core;
    using EA.Weee.Domain.Evidence;
    using EA.Weee.Requests.Admin;
    using FluentAssertions;
    using NUnit.Specifications;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using NoteStatus = Core.AatfEvidence.NoteStatus;
    using NoteStatusDomain = Domain.Evidence.NoteStatus;

    public class GetComplianceYearsFilterHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIGetAListOfComplianceYearsForAllowedStatuses : GetComplianceYearsFilterHandlerTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var evidenceNoteApprovedStatus = EvidenceNoteDbSetup.Init()
                  .With(n =>
                 {
                    n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                    n.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString(), SystemTime.UtcNow);
                 })
                 .WithComplianceYear(complianceYearMinus1)
                 .Create();

                var transferNoteApprovedStatus = TransferEvidenceNoteDbSetup.Init()
                 .With(n =>
                 {
                     n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                     n.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString(), SystemTime.UtcNow);
                 })
                 .WithComplianceYear(complianceYearMinus2)
                 .Create();

                var transferNoteSubmittedStatus = TransferEvidenceNoteDbSetup.Init()
                 .With(n =>
                 {
                     n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                 })
                 .WithComplianceYear(complianceYearMinus5)
                 .Create();

                var evidenceNoteSubmittedStatus = EvidenceNoteDbSetup.Init()
                 .With(n =>
                 {
                     n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                 })
                 .WithComplianceYear(complianceYearMinus3)
                 .Create();

                var transferNoteReturnedStatus = TransferEvidenceNoteDbSetup.Init()
                .With(n =>
                 {
                    n.UpdateStatus(NoteStatusDomain.Returned, UserId.ToString(), SystemTime.UtcNow);
                 })
                 .WithComplianceYear(complianceYearMinus4)
                 .Create();

                var evidenceNoteVoidStatus = EvidenceNoteDbSetup.Init()
                 .With(n =>
                 {
                     n.UpdateStatus(NoteStatusDomain.Void, UserId.ToString(), SystemTime.UtcNow);
                 })
                 .WithComplianceYear(complianceYear)
                 .Create();

                notesSet.Add(evidenceNoteApprovedStatus);
                notesSet.Add(transferNoteApprovedStatus);
                notesSet.Add(transferNoteSubmittedStatus);
                notesSet.Add(evidenceNoteSubmittedStatus);
                notesSet.Add(transferNoteReturnedStatus);
                notesSet.Add(evidenceNoteVoidStatus);

                request = new GetComplianceYearsFilter(allowedStatuses);
            };

            private readonly Because of = () =>
            {
                complianceYearList = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldReturnListOfComplianceYears = () =>
            {
                complianceYearList.Should().NotBeNull();
                complianceYearList.Should().NotBeEmpty();
            };

            private readonly It shouldHaveExpectedComplianceYears = () =>
            {
                complianceYearList.Count().Should().Be(6);
            };

            private readonly It shouldContainExpectedReturnedComplianceYearsInOrder = () =>
            {
                var toList = complianceYearList.ToList();
                toList[0].Should().Be(complianceYear);
                toList[1].Should().Be(complianceYearMinus1);
                toList[2].Should().Be(complianceYearMinus2);
                toList[3].Should().Be(complianceYearMinus3);
                toList[4].Should().Be(complianceYearMinus4);
            };
        }

        [Component]
        public class WhenIGetAnEmptyListOfComplianceYearsForStatusesNotAllowed : GetComplianceYearsFilterHandlerTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var draftNote1 = EvidenceNoteDbSetup.Init()
                 .WithComplianceYear(complianceYearMinus1)
                 .Create();

                var draftNote2 = TransferEvidenceNoteDbSetup.Init()
                 .WithComplianceYear(complianceYearMinus2)
                 .Create();

                var draftNote3 = TransferEvidenceNoteDbSetup.Init()
                 .WithComplianceYear(complianceYearMinus5)
                 .Create();

                notesSet.Add(draftNote1);
                notesSet.Add(draftNote2);
                notesSet.Add(draftNote3);
               
                request = new GetComplianceYearsFilter(allowedStatuses);
            };

            private readonly Because of = () =>
            {
                complianceYearList = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldReturnANotNullListOfComplianceYears = () =>
            {
                complianceYearList.Should().NotBeNull();
            };

            private readonly It shouldReturnAnEmptyListOfComplianceYears = () =>
            {
                complianceYearList.Should().BeEmpty();
            };
        }

        public class GetComplianceYearsFilterHandlerTestBase : WeeeContextSpecification
        {
            protected static IEnumerable<int> complianceYearList;
            protected static List<Note> notesSet;
            protected static List<NoteStatus> allowedStatuses;
            protected static List<NoteStatus> notAllowedStatuses;
            protected static IRequestHandler<GetComplianceYearsFilter, IEnumerable<int>> handler;
            protected static Fixture fixture;
            protected static GetComplianceYearsFilter request;

            protected static int complianceYear;
            protected static int complianceYearMinus1;
            protected static int complianceYearMinus2;
            protected static int complianceYearMinus3;
            protected static int complianceYearMinus4;
            protected static int complianceYearMinus5;

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
                handler = Container.Resolve<IRequestHandler<GetComplianceYearsFilter, IEnumerable<int>>>();

                complianceYear = SystemTime.Now.Year;
                complianceYearMinus1 = complianceYear - 1;
                complianceYearMinus2 = complianceYear - 2;
                complianceYearMinus3 = complianceYear - 3;
                complianceYearMinus4 = complianceYear - 4;
                complianceYearMinus5 = complianceYear - 5;
            }
        }
    }
}
