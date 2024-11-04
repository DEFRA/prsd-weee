namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations.DirectRegistrants
{
    using AutoFixture;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Domain.Producer;
    using EA.Weee.RequestHandlers.Organisations.DirectRegistrants;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.RequestHandlers.Shared;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;

    public class GetSmallProducerSubmissionHandlerTests : SimpleUnitTestBase
    {
        private readonly IWeeeAuthorization authorization;
        private readonly ISmallProducerSubmissionService smallProducerSubmissionService;
        private readonly GetSmallProducerSubmissionHandler handler;
        private readonly Guid directRegistrantId = Guid.NewGuid();
        private readonly ISmallProducerDataAccess smallProducerDataAccess;

        public GetSmallProducerSubmissionHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            smallProducerSubmissionService = A.Fake<ISmallProducerSubmissionService>();
            smallProducerDataAccess = A.Fake<ISmallProducerDataAccess>();

            handler = new GetSmallProducerSubmissionHandler(authorization, smallProducerSubmissionService, smallProducerDataAccess);
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

        [Fact]
        public async Task HandleAsync_CallsSmallProducerService_ReturnsResult()
        {
            // Arrange
            var request = new GetSmallProducerSubmission(directRegistrantId);
            var directRegistrant = SetupValidDirectRegistrant();
            var smallProducerData = TestFixture.Create<SmallProducerSubmissionData>();

            A.CallTo(() => smallProducerSubmissionService.GetSmallProducerSubmissionData(directRegistrant, false)).Returns(smallProducerData);

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            result.Should().Be(smallProducerData);
        }

        private DirectRegistrant SetupValidDirectRegistrant()
        {
            var directRegistrant = A.Fake<DirectRegistrant>();
            A.CallTo(() => directRegistrant.OrganisationId).Returns(Guid.NewGuid());
            A.CallTo(() => directRegistrant.Organisation).Returns(A.Fake<Organisation>());
            A.CallTo(() => directRegistrant.Id).Returns(directRegistrantId);

            A.CallTo(() => smallProducerDataAccess.GetById(directRegistrantId))
                .Returns(Task.FromResult(directRegistrant));

            return directRegistrant;
        }
    }
}