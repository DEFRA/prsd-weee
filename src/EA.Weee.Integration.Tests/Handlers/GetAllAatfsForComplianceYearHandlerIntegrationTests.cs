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
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.Evidence;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Requests.Admin;
    using FluentAssertions;
    using NUnit.Specifications;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using NoteStatusDomain = Domain.Evidence.NoteStatus;
    using UserStatus = Domain.User.UserStatus;

    public class GetAllAatfsForComplianceYearHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class GetAllAatfsForComplianceForSelectedComplianceYear : GetAllAatfsForComplianceYearHandlerTestBase
        {
            private static Aatf aatf1;
            private static Aatf aatf2;
            private static Aatf aatf3;
            private static Aatf aatf4;

            private readonly Establish context = () =>
            {
                LocalSetup();

                var complianceYear = 1;

                organisation = OrganisationDbSetup.Init().Create();
                aatf1 = AatfDbSetup.Init().WithOrganisation(organisation.Id).Create();
                aatf2 = AatfDbSetup.Init().WithOrganisation(organisation.Id).Create();
                aatf3 = AatfDbSetup.Init().WithOrganisation(organisation.Id).Create();
                aatf4 = AatfDbSetup.Init().WithOrganisation(organisation.Id).Create();

                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, organisation.Id)
                    .WithStatus(UserStatus.Active)
                    .Create();

                var evidence1 = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                    })
                    .WithComplianceYear(complianceYear)
                    .WithAatf(aatf1.Id)
                    .Create();

                var evidence2 = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                        n.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString(), SystemTime.UtcNow);
                    })
                    .WithComplianceYear(complianceYear)
                    .WithAatf(aatf2.Id)
                    .Create();

                var evidence3 = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                        n.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString(), SystemTime.UtcNow);
                        n.UpdateStatus(NoteStatusDomain.Void, UserId.ToString(), SystemTime.UtcNow);
                    })
                    .WithComplianceYear(complianceYear)
                    .WithAatf(aatf3.Id)
                    .Create();

                var evidence4 = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                        n.UpdateStatus(NoteStatusDomain.Returned, UserId.ToString(), SystemTime.UtcNow);
                    })
                    .WithComplianceYear(complianceYear)
                    .WithAatf(aatf3.Id)
                    .Create();

                var evidence5 = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                        n.UpdateStatus(NoteStatusDomain.Rejected, UserId.ToString(), SystemTime.UtcNow);
                    })
                    .WithComplianceYear(complianceYear)
                    .WithAatf(aatf3.Id)
                    .Create();

                // draft note so aatf should not be in the results
                EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .WithComplianceYear(complianceYear)
                    .WithAatf(aatf4.Id)
                    .Create();

                notesSet.Add(evidence1);
                notesSet.Add(evidence2);
                notesSet.Add(evidence3);
                notesSet.Add(evidence4);
                notesSet.Add(evidence5);

                aatfSet.Add(aatf1);
                aatfSet.Add(aatf2);
                aatfSet.Add(aatf3);

                request = new GetAllAatfsForComplianceYearRequest(complianceYear);
            };

            private readonly Because of = () =>
            {
                listOfEntityIdDisplayNameData = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldReturnListOfEntityIdDisplayNameData = () =>
            {
                listOfEntityIdDisplayNameData.Should().NotBeNull();
            };

            private readonly It shouldHaveExpectedResultsCountToBeSetOfAatfsNotes = () =>
            {
                listOfEntityIdDisplayNameData.Should().HaveCount(aatfSet.Count);
            };

            private readonly It shouldHaveExpectedData = () =>
            {
                var resultAatfIds = listOfEntityIdDisplayNameData.Select(a => a.Id).ToList();
                var originalAatfIds = aatfSet.Select(a => a.Id).ToList();

                resultAatfIds.Should().BeEquivalentTo(originalAatfIds);
            };

            private readonly It shouldHaveExcludedExpectedData = () =>
            {
                listOfEntityIdDisplayNameData.FirstOrDefault(e => e.Id == aatf4.Id).Should().BeNull();
            };

            private readonly It shouldHaveDistinctItems = () =>
            {
                listOfEntityIdDisplayNameData.Should().OnlyHaveUniqueItems();
            };

            private readonly It shouldBeOrderedCorrectly = () =>
            {
                listOfEntityIdDisplayNameData.Should().BeInAscendingOrder(e => e.DisplayName);
            };
        }

        public class GetAllAatfsForComplianceYearHandlerTestBase : WeeeContextSpecification
        {
            protected static List<EntityIdDisplayNameData> listOfEntityIdDisplayNameData;
            protected static Organisation organisation;
            protected static List<Note> notesSet;
            protected static List<Aatf> aatfSet;
            protected static IRequestHandler<GetAllAatfsForComplianceYearRequest, List<EntityIdDisplayNameData>> handler;
            protected static Fixture fixture;
            protected static GetAllAatfsForComplianceYearRequest request;
   
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
                aatfSet = new List<Aatf>();
                handler = Container.Resolve<IRequestHandler<GetAllAatfsForComplianceYearRequest, List<EntityIdDisplayNameData>>>();
            }
        }
    }
}
