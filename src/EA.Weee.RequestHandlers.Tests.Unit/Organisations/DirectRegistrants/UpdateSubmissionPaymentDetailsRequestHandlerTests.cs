namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations.DirectRegistrants
{
    using AutoFixture;
    using EA.Prsd.Core;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Helpers;
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

    public class UpdateSubmissionPaymentDetailsRequestHandlerTests : SimpleUnitTestBase
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly WeeeContext weeeContext;
        private readonly IPaymentSessionDataAccess paymentSessionDataAccess;
        private readonly UpdateSubmissionPaymentDetailsRequestHandler handler;
        private readonly Guid directRegistrantId = Guid.NewGuid();
        private readonly Guid paymentSessionId = Guid.NewGuid();
        private readonly Guid organisationId = Guid.NewGuid();
        private readonly int year;

        private DirectProducerSubmission currentYearSubmission;
        public UpdateSubmissionPaymentDetailsRequestHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            weeeContext = A.Fake<WeeeContext>();
            var systemDataAccess = A.Fake<ISystemDataDataAccess>();
            paymentSessionDataAccess = A.Fake<IPaymentSessionDataAccess>();

            year = 2024;

            A.CallTo(() => systemDataAccess.GetSystemDateTime()).Returns(new DateTime(year, 1, 1));

            handler = new UpdateSubmissionPaymentDetailsRequestHandler(
                authorization,
                genericDataAccess,
                weeeContext,
                systemDataAccess,
                systemDataAccess,
                paymentSessionDataAccess);
        }

        [Fact]
        public async Task HandleAsync_AuthorisationCheck_IsCalled()
        {
            // Arrange
            var request = CreateValidRequest();
            SetupValidPaymentSession();
            SetupValidDirectRegistrant();

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
            SetupValidDirectRegistrant();

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

            var paymentSession = SetupValidPaymentSession();

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => authorization.EnsureOrganisationAccess(organisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_WhenPaymentSessionNotFound_ThrowsInvalidOperationException()
        {
            // Arrange
            var request = CreateValidRequest();
            SetupValidDirectRegistrant();
            SetupNonExistentPaymentSession();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await handler.HandleAsync(request));
        }

        [Fact]
        public async Task HandleAsync_UpdatesPaymentSessionStatus()
        {
            // Arrange
            var request = CreateValidRequest();
            SetupValidDirectRegistrant();
            var paymentSession = SetupValidPaymentSession();

            // Act
            await handler.HandleAsync(request);

            // Assert
            paymentSession.Status.Should().Be(request.PaymentStatus.ToDomainEnumeration<PaymentState>());
        }

        [Fact]
        public async Task HandleAsync_UpdatesPaymentSessionInFinalState()
        {
            // Arrange
            var request = CreateValidRequest();
            SetupValidDirectRegistrant();
            var paymentSession = SetupValidPaymentSession();

            // Act
            await handler.HandleAsync(request);

            // Assert
            paymentSession.InFinalState.Should().Be(request.IsFinalState);
        }

        [Fact]
        public async Task HandleAsync_WhenIsFinalState_UpdatesFinalPaymentSession()
        {
            // Arrange
            var request = CreateValidRequest(isFinalState: true);
            var directRegistrant = SetupValidDirectRegistrant();
            var paymentSession = SetupValidPaymentSession();

            // Act
            await handler.HandleAsync(request);

            // Assert
            currentYearSubmission.FinalPaymentSession.Should().Be(paymentSession);
        }

        [Fact]
        public async Task HandleAsync_WhenNotFinalState_DoesNotUpdateFinalPaymentSession()
        {
            // Arrange
            var request = CreateValidRequest(isFinalState: false);
            var directRegistrant = SetupValidDirectRegistrant();
            SetupValidPaymentSession();

            // Act
            await handler.HandleAsync(request);

            // Assert
            currentYearSubmission.FinalPaymentSession.Should().BeNull();
        }

        [Fact]
        public async Task HandleAsync_CallsSaveChangesAsync()
        {
            // Arrange
            var request = CreateValidRequest();
            SetupValidDirectRegistrant();
            SetupValidPaymentSession();

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => weeeContext.SaveChangesAsync()).MustHaveHappenedOnceExactly();
        }

        private UpdateSubmissionPaymentDetailsRequest CreateValidRequest(bool isFinalState = false)
        {
            return new UpdateSubmissionPaymentDetailsRequest(
                directRegistrantId,
                TestFixture.Create<PaymentStatus>(),
                paymentSessionId,
                isFinalState);
        }

        private DirectRegistrant SetupValidDirectRegistrant()
        {
            var directRegistrant = A.Fake<DirectRegistrant>();
            A.CallTo(() => directRegistrant.OrganisationId).Returns(organisationId);
            currentYearSubmission = new DirectProducerSubmission(A.Fake<RegisteredProducer>(), year);
            var directProducerSubmissionNotCurrentYear = new DirectProducerSubmission(A.Fake<RegisteredProducer>(), SystemTime.UtcNow.Year + 1);

            currentYearSubmission.CurrentSubmission =
                new DirectProducerSubmissionHistory(currentYearSubmission, null, null);

            currentYearSubmission.DirectRegistrant = directRegistrant;

            directRegistrant.DirectProducerSubmissions.Add(currentYearSubmission);
            directRegistrant.DirectProducerSubmissions.Add(directProducerSubmissionNotCurrentYear);

            A.CallTo(() => genericDataAccess.GetById<DirectRegistrant>(directRegistrantId))
                .Returns(Task.FromResult(directRegistrant));

            return directRegistrant;
        }

        private void SetupNonExistentPaymentSession()
        {
            A.CallTo(() => paymentSessionDataAccess.GetByIdAsync(paymentSessionId))
                .Returns(Task.FromResult<PaymentSession>(null));
        }

        private PaymentSession SetupValidPaymentSession()
        {
            var paymentSession = A.Fake<PaymentSession>();
            A.CallTo(() => paymentSessionDataAccess.GetByIdAsync(paymentSessionId))
                .Returns(Task.FromResult(paymentSession));

            return paymentSession;
        }
    }
}