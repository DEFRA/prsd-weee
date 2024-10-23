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
    using Prsd.Core.Domain;
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;

    public class AddPaymentSessionRequestHandlerTests : SimpleUnitTestBase
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly ISmallProducerDataAccess smallProducerDataAccess;
        private readonly WeeeContext weeeContext;
        private readonly AddPaymentSessionRequestHandler handler;
        private readonly Guid directRegistrantId = Guid.NewGuid();
        private readonly Guid userId = Guid.NewGuid();
        private DirectProducerSubmission currentYearSubmission;

        public AddPaymentSessionRequestHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            weeeContext = A.Fake<WeeeContext>();
            smallProducerDataAccess = A.Fake<ISmallProducerDataAccess>();
            var systemDataAccess = A.Fake<ISystemDataDataAccess>();
            var userContext = A.Fake<IUserContext>();

            A.CallTo(() => systemDataAccess.GetSystemDateTime()).Returns(SystemTime.UtcNow);
            A.CallTo(() => userContext.UserId).Returns(userId);

            handler = new AddPaymentSessionRequestHandler(
                authorization,
                genericDataAccess,
                weeeContext,
                systemDataAccess,
                userContext,
                smallProducerDataAccess);
        }

        [Fact]
        public async Task HandleAsync_AuthorisationCheck_IsCalled()
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
        public async Task HandleAsync_AuthorisationCheck_NotAuthorized_ThrowsSecurityException()
        {
            // Arrange
            var request = CreateValidRequest();
            A.CallTo(() => authorization.EnsureCanAccessExternalArea()).Throws<SecurityException>();

            // Act & Assert
            await Assert.ThrowsAsync<SecurityException>(
                async () => await handler.HandleAsync(request));
        }

        [Fact]
        public async Task HandleAsync_WhenDirectRegistrantNotFound_ThrowsNullReferenceException()
        {
            // Arrange
            var request = CreateValidRequest();
            A.CallTo(() => genericDataAccess.GetById<DirectRegistrant>(request.DirectRegistrantId))
                .Returns(Task.FromResult<DirectRegistrant>(null));

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => handler.HandleAsync(request));
        }

        [Fact]
        public async Task HandleAsync_WhenFinalPaymentSessionExists_ThrowsInvalidOperationException()
        {
            // Arrange
            var request = CreateValidRequest();
            var directRegistrant = SetupValidDirectRegistrant(true);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleAsync(request));
        }

        [Fact]
        public async Task HandleAsync_AddsNewPaymentSession()
        {
            // Arrange
            var request = CreateValidRequest();
            var directRegistrant = SetupValidDirectRegistrant();

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => weeeContext.PaymentSessions.Add(A<PaymentSession>.That.Matches(p => p.InFinalState == false &&
                                                                                                            p.Amount == request.Amount &&
                                                                                                            p.DirectProducerSubmission == currentYearSubmission &&
                                                                                                            p.DirectRegistrant == directRegistrant &&
                                                                                                            p.PaymentReturnToken == request.PaymentReturnToken &&
                                                                                                            p.PaymentId == request.PaymentId &&
                                                                                                            p.PaymentReference == request.PaymentReference &&
                                                                                                            p.Status == PaymentState.New))).MustHaveHappenedOnceExactly()
                .Then(A.CallTo(() => weeeContext.SaveChangesAsync()).MustHaveHappenedOnceExactly());
        }

        private AddPaymentSessionRequest CreateValidRequest()
        {
            return new AddPaymentSessionRequest(
                directRegistrantId,
                TestFixture.Create<string>(),
                TestFixture.Create<string>(),
                TestFixture.Create<string>(),
                TestFixture.Create<decimal>());
        }

        private DirectRegistrant SetupValidDirectRegistrant(bool hasFinalPaymentSession = false)
        {
            var directRegistrant = A.Fake<DirectRegistrant>();
            A.CallTo(() => directRegistrant.Id).Returns(directRegistrantId);
            currentYearSubmission = new DirectProducerSubmission(A.Fake<RegisteredProducer>(), SystemTime.UtcNow.Year);
            var directProducerSubmissionNotCurrentYear = new DirectProducerSubmission(A.Fake<RegisteredProducer>(), SystemTime.UtcNow.Year + 1);

            currentYearSubmission.CurrentSubmission =
                new DirectProducerSubmissionHistory(currentYearSubmission, null, null);

            currentYearSubmission.DirectRegistrant = directRegistrant;

            if (hasFinalPaymentSession)
            {
                currentYearSubmission.PaymentFinished = true;
            }

            directRegistrant.DirectProducerSubmissions.Add(currentYearSubmission);
            directRegistrant.DirectProducerSubmissions.Add(directProducerSubmissionNotCurrentYear);

            A.CallTo(() => genericDataAccess.GetById<DirectRegistrant>(directRegistrantId))
                .Returns(Task.FromResult(directRegistrant));

            A.CallTo(() =>
                smallProducerDataAccess.GetCurrentDirectRegistrantSubmissionByComplianceYear(directRegistrantId,
                    SystemTime.UtcNow.Year)).Returns(currentYearSubmission);

            return directRegistrant;
        }
    }
}