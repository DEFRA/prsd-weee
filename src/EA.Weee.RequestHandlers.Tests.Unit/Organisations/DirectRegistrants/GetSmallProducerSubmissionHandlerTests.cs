namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations.DirectRegistrants
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Organisations;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Domain.Producer;
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

        public GetSmallProducerSubmissionHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            mapper = A.Fake<IMapper>();

            handler = new GetSmallProducerSubmissionHandler(authorization, genericDataAccess, mapper);
        }

        [Fact]
        public async Task HandleAsync_AuthorizationCheck_IsCalled()
        {
            // Arrange
            var request = new GetSmallProducerSubmission(Guid.NewGuid());
            SetupValidDirectRegistrant(request.DirectRegistrantId);

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => authorization.EnsureCanAccessExternalArea()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_AuthorizationCheck_NotAuthorized_ThrowsSecurityException()
        {
            // Arrange
            var request = new GetSmallProducerSubmission(Guid.NewGuid());
            A.CallTo(() => authorization.EnsureCanAccessExternalArea()).Throws<SecurityException>();

            // Act & Assert
            await Assert.ThrowsAsync<SecurityException>(
                async () => await handler.HandleAsync(request));
        }

        [Fact]
        public async Task HandleAsync_EnsureOrganisationAccess_IsCalled()
        {
            // Arrange
            var request = new GetSmallProducerSubmission(Guid.NewGuid());
            var directRegistrant = SetupValidDirectRegistrant(request.DirectRegistrantId);

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => authorization.EnsureOrganisationAccess(directRegistrant.OrganisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_WhenCurrentYearSubmissionExists_ReturnsSmallProducerSubmissionData()
        {
            // Arrange
            var request = new GetSmallProducerSubmission(Guid.NewGuid());
            var directRegistrant = SetupValidDirectRegistrant(request.DirectRegistrantId, true);
            var organisationData = A.Fake<OrganisationData>();
            A.CallTo(() => mapper.Map<Organisation, OrganisationData>(directRegistrant.Organisation)).Returns(organisationData);

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.OrganisationData.Should().Be(organisationData);
            result.CurrentSubmission.Should().NotBeNull();
        }

        [Fact]
        public async Task HandleAsync_WhenNoCurrentYearSubmission_ReturnsNull()
        {
            // Arrange
            var request = new GetSmallProducerSubmission(Guid.NewGuid());
            SetupValidDirectRegistrant(request.DirectRegistrantId, false);

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            result.Should().BeNull();
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public async Task HandleAsync_WhenCurrentYearSubmissionExists_SetsOrganisationDetailsCompleteCorrectly(bool hasBusinessAddressId, bool expectedOrganisationDetailsComplete)
        {
            // Arrange
            var request = new GetSmallProducerSubmission(Guid.NewGuid());
            var directRegistrant = SetupValidDirectRegistrant(request.DirectRegistrantId, true);
            var organisationData = A.Fake<OrganisationData>();
            A.CallTo(() => mapper.Map<Organisation, OrganisationData>(directRegistrant.Organisation)).Returns(organisationData);
            
            var currentYearSubmission = A.Fake<DirectProducerSubmissionHistory>();

            A.CallTo(() => currentYearSubmission.BusinessAddressId).Returns(hasBusinessAddressId ? (Guid?)Guid.NewGuid() : null);
            A.CallTo(() => directRegistrant.DirectProducerSubmissions.ElementAt(0).CurrentSubmission)
                .Returns(currentYearSubmission);

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.CurrentSubmission.Should().NotBeNull();
            result.CurrentSubmission.OrganisationDetailsComplete.Should().Be(expectedOrganisationDetailsComplete);
        }

        private DirectRegistrant SetupValidDirectRegistrant(Guid directRegistrantId, bool hasCurrentYearSubmission = false)
        {
            var directRegistrant = A.Fake<DirectRegistrant>();
            A.CallTo(() => directRegistrant.OrganisationId).Returns(Guid.NewGuid());
            A.CallTo(() => directRegistrant.Organisation).Returns(A.Fake<Organisation>());

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