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
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain.AatfReturn;
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
    using UserStatus = Domain.User.UserStatus;
    using WasteType = Core.AatfEvidence.WasteType;

    public class GetAllAatfsForComplianceYearHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIGetAListOfEntityIdDisplayNameDataWitSelectedComplianceYear : GetAllAatfsForComplianceYearHandlerTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var complianceYear = 1;

                organisation = OrganisationDbSetup.Init().Create();
                aatf1 = AatfDbSetup.Init().WithOrganisation(organisation.Id).Create();
                aatf2 = AatfDbSetup.Init().WithOrganisation(organisation.Id).Create();
                aatf3 = AatfDbSetup.Init().WithOrganisation(organisation.Id).Create();

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
                    })
                    .WithComplianceYear(complianceYear)
                    .WithAatf(aatf2.Id)
                    .Create();

                var evidence3 = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                    })
                    .WithComplianceYear(complianceYear)
                    .WithAatf(aatf3.Id)
                    .Create();

                notesSet.Add(evidence1);
                notesSet.Add(evidence2);
                notesSet.Add(evidence3);

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

            private readonly It shouldHaveExpectedResultsCountToBeSetOfNotes = () =>
            {
                listOfEntityIdDisplayNameData.Should().HaveCount(notesSet.Count);
                listOfEntityIdDisplayNameData.Count.Should().Be(notesSet.Count);
            };

            private readonly It shouldHaveExpectedData = () =>
            {
                var resultAatfIds = listOfEntityIdDisplayNameData.Select(a => a.Id).ToList();
                var originalAatfIds = aatfSet.Select(a => a.Id).ToList();

                resultAatfIds.Should().BeEquivalentTo(originalAatfIds);
            };
        }

        [Component]
        public class WhenIGetAnEmptyListOfEntityIdDisplayNameDataForAComplianceYear : GetAllAatfsForComplianceYearHandlerTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                aatf1 = AatfDbSetup.Init().WithOrganisation(organisation.Id).Create();
                aatf2 = AatfDbSetup.Init().WithOrganisation(organisation.Id).Create();

                OrganisationUserDbSetup.Init()
                    .WithUserIdAndOrganisationId(UserId, organisation.Id)
                    .WithStatus(UserStatus.Active)
                    .Create();

                var evidence1 = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                    })
                    .WithComplianceYear(1)
                    .WithAatf(aatf1.Id)
                    .Create();

                var evidence2 = EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                    })
                    .WithComplianceYear(2)
                    .WithAatf(aatf2.Id)
                    .Create();

                notesSet.Add(evidence1);
                notesSet.Add(evidence2);

                request = new GetAllAatfsForComplianceYearRequest(3);
            };

            private readonly Because of = () =>
            {
                listOfEntityIdDisplayNameData = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldReturnAnEmptyListOfEntityIdDisplayNameData = () =>
            {
                listOfEntityIdDisplayNameData.Should().NotBeNull();
                listOfEntityIdDisplayNameData.Should().BeEmpty();
            };

            private readonly It shouldHaveZeroCount = () =>
            {
                listOfEntityIdDisplayNameData.Should().HaveCount(0);
            };
        }

        public class GetAllAatfsForComplianceYearHandlerTestBase : WeeeContextSpecification
        {
            protected static List<EntityIdDisplayNameData> listOfEntityIdDisplayNameData;
            protected static Organisation organisation;
            protected static Aatf aatf1;
            protected static Aatf aatf2;
            protected static Aatf aatf3;
            protected static List<Note> notesSet;
            protected static List<Aatf> aatfSet;
            protected static IRequestHandler<GetAllAatfsForComplianceYearRequest, List<EntityIdDisplayNameData>> handler;
            protected static Fixture fixture;
            protected static GetAllAatfsForComplianceYearRequest request;
   
            public static void LocalSetup()
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings(resetData: true)
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
