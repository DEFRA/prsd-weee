namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations.DirectRegistrants
{
    using AutoFixture;
    using EA.Prsd.Core;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.DataAccess;
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

    public class ValidateAndGetSubmissionPaymentHandlerTests : SimpleUnitTestBase
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext weeeContext;
        private readonly ISystemDataDataAccess systemDataAccess;
        private readonly IPaymentSessionDataAccess paymentSessionDataAccess;
        private readonly ValidateAndGetSubmissionPaymentHandler handler;
        private readonly Guid directRegistrantId = Guid.NewGuid();
        private readonly Guid organisationId = Guid.NewGuid();
        private const string PaymentReturnToken = "test-token";
        private readonly int year;

        public ValidateAndGetSubmissionPaymentHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            weeeContext = A.Fake<WeeeContext>();
            systemDataAccess = A.Fake<ISystemDataDataAccess>();
            paymentSessionDataAccess = A.Fake<IPaymentSessionDataAccess>();

            year = 2024;

            A.CallTo(() => systemDataAccess.GetSystemDateTime()).Returns(new DateTime(year, 1, 1));

            handler = new ValidateAndGetSubmissionPaymentHandler(
                authorization,
                weeeContext,
                systemDataAccess,
                paymentSessionDataAccess);
        }

        [Fact]
        public async Task HandleAsync_AuthorisationCheck_IsCalled()
        {
            // Arrange
            var request = CreateValidRequest();
            SetupValidPaymentSession();

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => authorization.EnsureCanAccessExternalArea()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_AuthorisationCheck_NotAuthorised_ThrowsSecurityException()
        {
            // Arrange
            var request = CreateValidRequest();
            A.CallTo(() => authorization.EnsureCanAccessExternalArea()).Throws<SecurityException>();

            // Act & Assert
            await Assert.ThrowsAsync<SecurityException>(
                async () => await handler.HandleAsync(request));
        }

        [Fact]
        public async Task HandleAsync_WhenNoPaymentRequestExists_ReturnsErrorMessage()
        {
            // Arrange
            var request = CreateValidRequest();
            SetupNoExistingPaymentSessions();

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.ErrorMessage.Should().Be($"No payment request exists {request.PaymentReturnToken}");
        }

        [Fact]
        public async Task HandleAsync_WhenNoCurrentInProgressPayment_ReturnsErrorMessage()
        {
            // Arrange
            var request = CreateValidRequest();
            SetupExistingPaymentSessions();
            SetupNoCurrentInProgressPayment();

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.ErrorMessage.Should().Be($"No payment request {request.PaymentReturnToken} exists for user");
        }

        [Fact]
        public async Task HandleAsync_WhenValidPaymentSession_ReturnsSubmissionPaymentDetails()
        {
            // Arrange
            var request = CreateValidRequest();
            var paymentSession = SetupValidPaymentSession();

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.DirectRegistrantId.Should().Be(paymentSession.DirectRegistrantId);
            result.PaymentId.Should().Be(paymentSession.PaymentId);
            result.PaymentReference.Should().Be(paymentSession.PaymentReference);
            result.PaymentSessionId.Should().Be(paymentSession.Id);
        }

        [Fact]
        public async Task HandleAsync_EnsureOrganisationAccess_IsCalled()
        {
            // Arrange
            var request = CreateValidRequest();
            var paymentSession = SetupValidPaymentSession();

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => authorization.EnsureOrganisationAccess(organisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_SaveChangesAsync_IsCalled()
        {
            // Arrange
            var request = CreateValidRequest();
            SetupValidPaymentSession();

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => weeeContext.SaveChangesAsync()).MustHaveHappenedOnceExactly();
        }

        private ValidateAndGetSubmissionPayment CreateValidRequest()
        {
            return new ValidateAndGetSubmissionPayment(PaymentReturnToken, directRegistrantId);
        }

        private void SetupNoExistingPaymentSessions()
        {
            A.CallTo(() => paymentSessionDataAccess.AnyPaymentTokenAsync(PaymentReturnToken))
                .Returns(Task.FromResult(false));
        }

        private void SetupExistingPaymentSessions()
        {
            A.CallTo(() => paymentSessionDataAccess.AnyPaymentTokenAsync(PaymentReturnToken))
                .Returns(Task.FromResult(true));
        }

        private void SetupNoCurrentInProgressPayment()
        {
            A.CallTo(() => paymentSessionDataAccess.GetCurrentInProgressPayment(PaymentReturnToken, directRegistrantId, year))
                .Returns(Task.FromResult<PaymentSession>(null));
        }

        private PaymentSession SetupValidPaymentSession()
        {
            SetupExistingPaymentSessions();

            var paymentSession = A.Fake<PaymentSession>();
            A.CallTo(() => paymentSession.DirectRegistrantId).Returns(directRegistrantId);
            A.CallTo(() => paymentSession.PaymentId).Returns(TestFixture.Create<string>());
            A.CallTo(() => paymentSession.PaymentReference).Returns(TestFixture.Create<string>());
            A.CallTo(() => paymentSession.Id).Returns(TestFixture.Create<Guid>());
            A.CallTo(() => paymentSession.DirectRegistrant.OrganisationId).Returns(organisationId);

            A.CallTo(() => paymentSessionDataAccess.GetCurrentInProgressPayment(PaymentReturnToken, directRegistrantId, year))
                .Returns(Task.FromResult(paymentSession));

            A.CallTo(() => systemDataAccess.GetSystemDateTime())
                .Returns(Task.FromResult(SystemTime.UtcNow));

            return paymentSession;
        }
    }
}