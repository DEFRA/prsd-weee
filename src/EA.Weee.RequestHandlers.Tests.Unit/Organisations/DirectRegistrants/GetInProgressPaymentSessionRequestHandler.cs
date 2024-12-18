namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations.DirectRegistrants
{
    using AutoFixture;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
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
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;

    public class GetInProgressPaymentSessionRequestHandlerTests : SimpleUnitTestBase
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly ISystemDataDataAccess systemDataDataAccess;
        private readonly IPaymentSessionDataAccess paymentSessionDataAccess;
        private readonly GetInProgressPaymentSessionRequestHandler handler;
        private readonly Guid directRegistrantId = Guid.NewGuid();
        private readonly Guid organisationId = Guid.NewGuid();
        private readonly int year;

        public GetInProgressPaymentSessionRequestHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            systemDataDataAccess = A.Fake<ISystemDataDataAccess>();
            paymentSessionDataAccess = A.Fake<IPaymentSessionDataAccess>();

            year = 2024;

            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(new DateTime(year, 12, 1));

            handler = new GetInProgressPaymentSessionRequestHandler(
                authorization,
                genericDataAccess,
                systemDataDataAccess,
                paymentSessionDataAccess);
        }

        [Fact]
        public async Task HandleAsync_AuthorizationCheck_IsCalled()
        {
            // Arrange
            var request = CreateValidRequest();
            SetupValidDirectRegistrant();

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => authorization.EnsureCanAccessExternalArea()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_AuthorizationCheck_NotAuthorized_ThrowsSecurityException()
        {
            // Arrange
            var request = CreateValidRequest();
            A.CallTo(() => authorization.EnsureCanAccessExternalArea()).Throws<SecurityException>();

            // Act & Assert
            await Assert.ThrowsAsync<SecurityException>(
                async () => await handler.HandleAsync(request));
        }

        [Fact]
        public async Task HandleAsync_EnsureOrganisationAccess_IsCalled()
        {
            // Arrange
            var request = CreateValidRequest();
            SetupValidDirectRegistrant();

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => authorization.EnsureOrganisationAccess(organisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_WhenNoInProgressPaymentSession_ReturnsNull()
        {
            // Arrange
            var request = CreateValidRequest();
            SetupValidDirectRegistrant();

            A.CallTo(() => paymentSessionDataAccess.GetCurrentRetryPayment(request.DirectRegistrantId, year))
                .Returns(Task.FromResult<PaymentSession>(null));

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task HandleAsync_WhenInProgressPaymentSessionExists_ReturnsSubmissionPaymentDetails()
        {
            // Arrange
            var request = CreateValidRequest();
            SetupValidDirectRegistrant();
            var paymentSession = A.Fake<PaymentSession>();
            var paymentId = TestFixture.Create<string>();
            var paymentSessionId = TestFixture.Create<Guid>();

            A.CallTo(() => paymentSession.Id).Returns(paymentSessionId);
            A.CallTo(() => paymentSession.PaymentId).Returns(paymentId);
            A.CallTo(() => paymentSessionDataAccess.GetCurrentRetryPayment(request.DirectRegistrantId, year))
                .Returns(Task.FromResult(paymentSession));

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.PaymentId.Should().Be(paymentId);
            result.PaymentSessionId.Should().Be(paymentSessionId);
        }

        private GetInProgressPaymentSessionRequest CreateValidRequest()
        {
            return new GetInProgressPaymentSessionRequest(directRegistrantId);
        }

        private void SetupValidDirectRegistrant()
        {
            var directRegistrant = A.Fake<DirectRegistrant>();
            A.CallTo(() => directRegistrant.OrganisationId).Returns(organisationId);

            A.CallTo(() => genericDataAccess.GetById<DirectRegistrant>(directRegistrantId))
                .Returns(Task.FromResult(directRegistrant));

            A.CallTo(() => systemDataDataAccess.GetSystemDateTime())
                .Returns(Task.FromResult(SystemTime.UtcNow));
        }
    }
}