namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.DirectRegistrants
{
    using AutoFixture;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Domain.Producer;
    using EA.Weee.RequestHandlers.Admin.DirectRegistrants;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.RequestHandlers.Shared;
    using EA.Weee.Requests.Admin.DirectRegistrants;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;

    public class GetSmallProducerSubmissionByRegistrationNumberHandlerTests : SimpleUnitTestBase
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IRegisteredProducerDataAccess registeredProducerDataAccess;
        private readonly ISmallProducerSubmissionService smallProducerSubmissionService;
        private readonly GetSmallProducerSubmissionByRegistrationNumberHandler handler;
        private const string RegisteredProducerNumber = "PRN";

        public GetSmallProducerSubmissionByRegistrationNumberHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            registeredProducerDataAccess = A.Fake<IRegisteredProducerDataAccess>();
            smallProducerSubmissionService = A.Fake<ISmallProducerSubmissionService>();

            handler = new GetSmallProducerSubmissionByRegistrationNumberHandler(authorization, registeredProducerDataAccess, smallProducerSubmissionService);
        }

        [Fact]
        public async Task HandleAsync_AuthorizationCheck_IsCalled()
        {
            var request = new GetSmallProducerSubmissionByRegistrationNumber(RegisteredProducerNumber);
            SetupValidDirectRegistrant();

            await handler.HandleAsync(request);

            A.CallTo(() => authorization.EnsureCanAccessInternalArea()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_AuthorizationCheck_NotAuthorized_ThrowsSecurityException()
        {
            var request = new GetSmallProducerSubmissionByRegistrationNumber(RegisteredProducerNumber);
            A.CallTo(() => authorization.EnsureCanAccessInternalArea()).Throws<SecurityException>();

            await Assert.ThrowsAsync<SecurityException>(async () => await handler.HandleAsync(request));
        }

        [Fact]
        public async Task HandleAsync_CallsSmallProducerService_ReturnsResult()
        {
            // Arrange
            var request = new GetSmallProducerSubmissionByRegistrationNumber(RegisteredProducerNumber);
            var directRegistrant = SetupValidDirectRegistrant();
            var smallProducerData = TestFixture.Create<SmallProducerSubmissionData>();

            A.CallTo(() => smallProducerSubmissionService.GetSmallProducerSubmissionData(directRegistrant, true)).Returns(smallProducerData);

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
            A.CallTo(() => registeredProducerDataAccess.GetDirectRegistrantByRegistration(RegisteredProducerNumber))
                .Returns(directRegistrant);

            return directRegistrant;
        }
    }
}