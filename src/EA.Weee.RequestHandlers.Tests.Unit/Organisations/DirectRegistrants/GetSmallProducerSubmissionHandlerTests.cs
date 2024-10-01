namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations.DirectRegistrants
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Domain.Producer;
    using EA.Weee.RequestHandlers.Mappings;
    using EA.Weee.RequestHandlers.Organisations.DirectRegistrants;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;

    public class GetSmallProducerSubmissionHandlerTests : SimpleUnitTestBase
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IMapper mapper;
        private readonly GetSmallProducerSubmissionHandler handler;
        private readonly Guid directRegistrantId = Guid.NewGuid();

        public GetSmallProducerSubmissionHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            mapper = A.Fake<IMapper>();
            var systemDataDataAccess = A.Fake<ISystemDataDataAccess>();

            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(SystemTime.UtcNow);
            handler = new GetSmallProducerSubmissionHandler(authorization, genericDataAccess, mapper, systemDataDataAccess);
        }

        [Fact]
        public async Task HandleAsync_AuthorizationCheck_IsCalled()
        {
            var request = new GetSmallProducerSubmission(directRegistrantId);
            SetupValidDirectRegistrant();

            await handler.HandleAsync(request);

            A.CallTo(() => authorization.EnsureCanAccessExternalArea()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_AuthorizationCheck_NotAuthorized_ThrowsSecurityException()
        {
            var request = new GetSmallProducerSubmission(directRegistrantId);
            A.CallTo(() => authorization.EnsureCanAccessExternalArea()).Throws<SecurityException>();

            await Assert.ThrowsAsync<SecurityException>(async () => await handler.HandleAsync(request));
        }

        [Fact]
        public async Task HandleAsync_EnsureOrganisationAccess_IsCalled()
        {
            var request = new GetSmallProducerSubmission(directRegistrantId);
            var directRegistrant = SetupValidDirectRegistrant();

            await handler.HandleAsync(request);

            A.CallTo(() => authorization.EnsureOrganisationAccess(directRegistrant.OrganisationId)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task HandleAsync_WhenCurrentYearSubmissionExists_ReturnsSmallProducerSubmissionData(bool hasAuthorisedRep)
        {
            var request = new GetSmallProducerSubmission(directRegistrantId);
            var authorisedRepId = hasAuthorisedRep ? Guid.NewGuid() : (Guid?)null;
            var directRegistrant = SetupValidDirectRegistrant(true, authorisedRepId);

            var organisationData = A.Fake<OrganisationData>();
            A.CallTo(() => mapper.Map<Organisation, OrganisationData>(directRegistrant.Organisation)).Returns(organisationData);

            var submissionHistoryData = A.Fake<SmallProducerSubmissionHistoryData>();
            A.CallTo(() => mapper.Map<SmallProducerSubmissionHistoryData>(A<DirectProducerSubmissionSource>._)).Returns(submissionHistoryData);

            var result = await handler.HandleAsync(request);

            result.Should().NotBeNull();
            result.DirectRegistrantId.Should().Be(directRegistrantId);
            result.HasAuthorisedRepresentitive.Should().Be(hasAuthorisedRep);
            result.OrganisationData.Should().Be(organisationData);
            result.CurrentSubmission.Should().Be(submissionHistoryData);
        }

        [Fact]
        public async Task HandleAsync_WhenNoCurrentYearSubmission_ReturnsNull()
        {
            var request = new GetSmallProducerSubmission(Guid.NewGuid());
            SetupValidDirectRegistrant(false);

            var result = await handler.HandleAsync(request);

            result.Should().BeNull();
        }

        [Fact]
        public async Task HandleAsync_WhenCurrentYearSubmissionExists_CallsMapperCorrectly()
        {
            var request = new GetSmallProducerSubmission(directRegistrantId);
            var directRegistrant = SetupValidDirectRegistrant(true);
            var organisationData = A.Fake<OrganisationData>();
            A.CallTo(() => mapper.Map<Organisation, OrganisationData>(directRegistrant.Organisation)).Returns(organisationData);

            var currentYearSubmission = directRegistrant.DirectProducerSubmissions.FirstOrDefault(s => s.ComplianceYear == SystemTime.UtcNow.Year);

            await handler.HandleAsync(request);

            A.CallTo(() => mapper.Map<SmallProducerSubmissionHistoryData>(A<DirectProducerSubmissionSource>.That.Matches(s => s.DirectRegistrant == directRegistrant && 
                s.DirectProducerSubmission == currentYearSubmission))).MustHaveHappenedOnceExactly();
        }

        private DirectRegistrant SetupValidDirectRegistrant(bool hasCurrentYearSubmission = false, Guid? hasAuthorisedRep = null)
        {
            var directRegistrant = A.Fake<DirectRegistrant>();
            A.CallTo(() => directRegistrant.OrganisationId).Returns(Guid.NewGuid());
            A.CallTo(() => directRegistrant.Organisation).Returns(A.Fake<Organisation>());
            A.CallTo(() => directRegistrant.Id).Returns(directRegistrantId);
            A.CallTo(() => directRegistrant.AuthorisedRepresentativeId).Returns(hasAuthorisedRep);

            var submissions = new List<DirectProducerSubmission>();
            if (hasCurrentYearSubmission)
            {
                var currentYearSubmission = A.Fake<DirectProducerSubmission>();
                A.CallTo(() => currentYearSubmission.ComplianceYear).Returns(SystemTime.UtcNow.Year);
                submissions.Add(currentYearSubmission);
            }
            A.CallTo(() => directRegistrant.DirectProducerSubmissions).Returns(submissions);

            A.CallTo(() => genericDataAccess.GetById<DirectRegistrant>(directRegistrantId))
                .Returns(Task.FromResult(directRegistrant));

            return directRegistrant;
        }
    }
}