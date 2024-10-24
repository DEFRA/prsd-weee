namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations.DirectRegistrants
{
    using AutoFixture;
    using EA.Weee.Core.DirectRegistrant;
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
        private readonly ISystemDataDataAccess systemDataAccess;
        private readonly IPaymentSessionDataAccess paymentSessionDataAccess;
        private readonly ISmallProducerDataAccess smallProducerDataAccess;
        private readonly ValidateAndGetSubmissionPaymentHandler handler;
        private readonly Guid directRegistrantId = Guid.NewGuid();
        private readonly Guid organisationId = Guid.NewGuid();
        private const string PaymentReturnToken = "test-token";
        private readonly int year;

        public ValidateAndGetSubmissionPaymentHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            systemDataAccess = A.Fake<ISystemDataDataAccess>();
            paymentSessionDataAccess = A.Fake<IPaymentSessionDataAccess>();
            smallProducerDataAccess = A.Fake<ISmallProducerDataAccess>();

            year = 2024;
            A.CallTo(() => systemDataAccess.GetSystemDateTime()).Returns(new DateTime(year, 1, 1));

            handler = new ValidateAndGetSubmissionPaymentHandler(
                authorization,
                systemDataAccess,
                paymentSessionDataAccess,
                smallProducerDataAccess);
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
        public async Task HandleAsync_WhenDirectRegistrantSubmissionNotFound_ThrowsInvalidOperationException()
        {
            // Arrange
            var request = CreateValidRequest();
            SetupExistingPaymentSessions();
            A.CallTo(() => smallProducerDataAccess.GetCurrentDirectRegistrantSubmissionByComplianceYear(
                directRegistrantId, year)).Returns<DirectProducerSubmission>(null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await handler.HandleAsync(request));
        }

        [Fact]
        public async Task HandleAsync_WhenNoCurrentInProgressPayment_ReturnsErrorMessage()
        {
            // Arrange
            var request = CreateValidRequest();
            SetupExistingPaymentSessions();
            SetupValidSubmission();
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
            result.PaymentFinished.Should().BeFalse();
        }

        [Fact]
        public async Task HandleAsync_WhenPaymentFinished_ReturnsCompletedPaymentDetails()
        {
            // Arrange
            var request = CreateValidRequest();
            SetupExistingPaymentSessions();
            var submission = SetupCompletedSubmission();

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.DirectRegistrantId.Should().Be(submission.DirectRegistrantId);
            result.PaymentReference.Should().Be(submission.FinalPaymentSession.PaymentReference);
            result.PaymentFinished.Should().BeTrue();
            result.PaymentStatus.Should().Be(PaymentStatus.Success);
        }

        [Fact]
        public async Task HandleAsync_EnsureOrganisationAccess_IsCalled()
        {
            // Arrange
            var request = CreateValidRequest();
            SetupValidPaymentSession();

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => authorization.EnsureOrganisationAccess(organisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_WhenOrganisationAccessDenied_ThrowsSecurityException()
        {
            // Arrange
            var request = CreateValidRequest();
            SetupValidPaymentSession();
            A.CallTo(() => authorization.EnsureOrganisationAccess(organisationId))
                .Throws<SecurityException>();

            // Act & Assert
            await Assert.ThrowsAsync<SecurityException>(
                async () => await handler.HandleAsync(request));
        }

        private ValidateAndGetSubmissionPayment CreateValidRequest()
        {
            return new ValidateAndGetSubmissionPayment(PaymentReturnToken, directRegistrantId);
        }

        private void SetupNoExistingPaymentSessions()
        {
            A.CallTo(() => paymentSessionDataAccess.AnyPaymentTokenAsync(PaymentReturnToken))
                .Returns(false);
        }

        private void SetupExistingPaymentSessions()
        {
            A.CallTo(() => paymentSessionDataAccess.AnyPaymentTokenAsync(PaymentReturnToken))
                .Returns(true);
        }

        private void SetupNoCurrentInProgressPayment()
        {
            A.CallTo(() => paymentSessionDataAccess.GetCurrentInProgressPayment(PaymentReturnToken, directRegistrantId, year))
                .Returns<PaymentSession>(null);
        }

        private DirectProducerSubmission SetupCompletedSubmission()
        {
            var submission = A.Fake<DirectProducerSubmission>();
            var finalSession = A.Fake<PaymentSession>();

            A.CallTo(() => submission.DirectRegistrantId).Returns(directRegistrantId);
            A.CallTo(() => submission.DirectRegistrant.OrganisationId).Returns(organisationId);
            A.CallTo(() => submission.PaymentFinished).Returns(true);
            A.CallTo(() => submission.FinalPaymentSession).Returns(finalSession);
            A.CallTo(() => finalSession.PaymentReference).Returns("REF-123");
            A.CallTo(() => finalSession.Status).Returns(PaymentState.Success);

            A.CallTo(() => smallProducerDataAccess.GetCurrentDirectRegistrantSubmissionByComplianceYear(
                directRegistrantId, year)).Returns(submission);

            return submission;
        }

        private void SetupValidSubmission()
        {
            var submission = A.Fake<DirectProducerSubmission>();
            A.CallTo(() => submission.DirectRegistrantId).Returns(directRegistrantId);
            A.CallTo(() => submission.DirectRegistrant.OrganisationId).Returns(organisationId);
            A.CallTo(() => submission.PaymentFinished).Returns(false);

            A.CallTo(() => smallProducerDataAccess.GetCurrentDirectRegistrantSubmissionByComplianceYear(
                directRegistrantId, year)).Returns(submission);
        }

        private PaymentSession SetupValidPaymentSession()
        {
            SetupExistingPaymentSessions();
            SetupValidSubmission();

            var paymentSession = A.Fake<PaymentSession>();
            A.CallTo(() => paymentSession.DirectRegistrantId).Returns(directRegistrantId);
            A.CallTo(() => paymentSession.PaymentId).Returns(TestFixture.Create<string>());
            A.CallTo(() => paymentSession.PaymentReference).Returns(TestFixture.Create<string>());
            A.CallTo(() => paymentSession.Id).Returns(TestFixture.Create<Guid>());

            A.CallTo(() => paymentSessionDataAccess.GetCurrentInProgressPayment(PaymentReturnToken, directRegistrantId, year))
                .Returns(paymentSession);

            return paymentSession;
        }
    }
}