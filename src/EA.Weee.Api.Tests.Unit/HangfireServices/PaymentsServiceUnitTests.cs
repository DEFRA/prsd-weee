namespace EA.Weee.Api.Tests.Unit.HangfireServices
{
    using AutoFixture;
    using EA.Prsd.Core;
    using EA.Weee.Api.Client;
    using EA.Weee.Api.Client.Models.Pay;
    using EA.Weee.Api.HangfireServices;
    using EA.Weee.Api.Services;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Helpers;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Producer;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using Serilog;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;
    using PaymentState = Domain.Producer.PaymentState;

    public class PaymentsServiceTests : SimpleUnitTestBase
    {
        private readonly ILogger logger;
        private readonly WeeeContext context;
        private readonly IWeeeTransactionAdapter transactionAdapter;
        private readonly IPayClient payClient;
        private readonly IPaymentSessionDataAccess paymentSessionDataAccess;
        private readonly PaymentsService paymentsService;
        private readonly ConfigurationService configurationService;

        public PaymentsServiceTests()
        {
            logger = A.Fake<ILogger>();
            context = A.Fake<WeeeContext>();
            transactionAdapter = A.Fake<IWeeeTransactionAdapter>();
            payClient = A.Fake<IPayClient>();
            paymentSessionDataAccess = A.Fake<IPaymentSessionDataAccess>();
            configurationService = A.Fake<ConfigurationService>();

            paymentsService = new PaymentsService(
                logger,
                context,
                transactionAdapter,
                payClient,
                paymentSessionDataAccess,
                configurationService);
        }

        [Fact]
        public async Task RunMopUpJob_ShouldProcessIncompletePayments()
        {
            // Arrange
            var jobId = Guid.NewGuid();

            var incompletePayments = new List<PaymentSession>
            {
                new PaymentSession(),
                new PaymentSession()
            };

            A.CallTo(() => paymentSessionDataAccess.GetIncompletePaymentSessions(configurationService.CurrentConfiguration.GovUkPayWindowMinutes, configurationService.CurrentConfiguration.GovUkPayLastProcessedMinutes))
                .Returns(incompletePayments);

            // Act
            await paymentsService.RunMopUpJob(jobId);

            // Assert
            A.CallTo(() => logger.Information(A<string>.That.Contains("Starting RunMopUpJob"))).MustHaveHappenedOnceExactly();
            A.CallTo(() => logger.Information(A<string>.That.Contains($"Found {incompletePayments.Count} incomplete payments to process"))).MustHaveHappenedOnceExactly();
            A.CallTo(() => logger.Information(A<string>.That.Contains("Completed RunMopUpJob"))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task RunMopUpJob_ShouldHandleExceptions()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            A.CallTo(() => paymentSessionDataAccess.GetIncompletePaymentSessions(configurationService.CurrentConfiguration.GovUkPayWindowMinutes, configurationService.CurrentConfiguration.GovUkPayLastProcessedMinutes))
                .Throws(new Exception("Test exception"));

            // Act
            await paymentsService.RunMopUpJob(jobId);

            // Assert
            A.CallTo(() => logger.Error(A<Exception>._, A<string>.That.Contains("Error in RunMopUpJob"))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ProcessPayment_ShouldHandleAlreadyProcessedPayment()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var payment = new PaymentSession()
            {
                InFinalState = true,
                DirectProducerSubmission = new DirectProducerSubmission()
            };

            var incompletePayments = new List<PaymentSession>()
            {
                payment
            };

            A.CallTo(() => paymentSessionDataAccess.GetByIdAsync(payment.Id))
                .Returns(payment);
            A.CallTo(() => paymentSessionDataAccess.GetIncompletePaymentSessions(configurationService.CurrentConfiguration.GovUkPayWindowMinutes, configurationService.CurrentConfiguration.GovUkPayLastProcessedMinutes))
                .Returns(incompletePayments);

            // Act
            await paymentsService.RunMopUpJob(jobId);

            // Assert
            A.CallTo(() => logger.Information(A<string>.That.Contains("already processed"))).MustHaveHappenedOnceExactly();
            A.CallTo(() => payClient.GetPaymentAsync(A<string>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task ProcessPayment_ShouldUpdatePaymentStatus()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            const string paymentId = "ref";

            var payment = new PaymentSession()
            {
                InFinalState = false,
                PaymentId = paymentId,
                DirectProducerSubmission = new DirectProducerSubmission()
            };

            var incompletePayments = new List<PaymentSession>()
            {
                payment
            };

            var paymentStatus = TestFixture.Build<PaymentWithAllLinks>()
                .With(p => p.State, new Api.Client.Models.Pay.PaymentState() { Finished = true, Status = PaymentStatus.Success }).Create();
            
            A.CallTo(() => paymentSessionDataAccess.GetByIdAsync(payment.Id))
                .Returns(payment);
            A.CallTo(() => payClient.GetPaymentAsync(payment.PaymentId))
                .Returns(paymentStatus);
            A.CallTo(() => paymentSessionDataAccess.GetIncompletePaymentSessions(configurationService.CurrentConfiguration.GovUkPayWindowMinutes, configurationService.CurrentConfiguration.GovUkPayLastProcessedMinutes))
                .Returns(incompletePayments);

            // Act
            await paymentsService.RunMopUpJob(jobId);

            // Assert
            payment.Status.Should().Be(paymentStatus.State.Status.ToDomainEnumeration<PaymentState>());
            payment.InFinalState.Should().BeTrue();
            payment.LastProcessedAt.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromSeconds(10));
            A.CallTo(() => context.SaveChangesAsync(default)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ProcessPayment_ShouldHandleNullGovUkPayment()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            const string paymentId = "ref";

            var payment = new PaymentSession()
            {
                InFinalState = false,
                PaymentId = paymentId,
                DirectProducerSubmission = new DirectProducerSubmission()
            };

            var incompletePayments = new List<PaymentSession>()
            {
                payment
            };

            var paymentStatus = TestFixture.Build<PaymentWithAllLinks>()
                .With(p => p.State, new Api.Client.Models.Pay.PaymentState() { Finished = true, Status = PaymentStatus.Success }).Create();

            A.CallTo(() => paymentSessionDataAccess.GetByIdAsync(payment.Id))
                .Returns(payment);
            A.CallTo(() => payClient.GetPaymentAsync(A<string>._)).Returns<PaymentWithAllLinks>(null);
            A.CallTo(() => paymentSessionDataAccess.GetIncompletePaymentSessions(configurationService.CurrentConfiguration.GovUkPayWindowMinutes, configurationService.CurrentConfiguration.GovUkPayLastProcessedMinutes))
                .Returns(incompletePayments);

            // Act
            await paymentsService.RunMopUpJob(jobId);

            // Assert
            payment.Status.Should().Be(PaymentState.Error);
            payment.InFinalState.Should().BeTrue();
            payment.LastProcessedAt.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromSeconds(10));
            A.CallTo(() => context.SaveChangesAsync(default)).MustHaveHappenedOnceExactly();
        }
    }
}