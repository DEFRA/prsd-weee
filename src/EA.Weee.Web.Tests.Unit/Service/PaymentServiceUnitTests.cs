﻿namespace EA.Weee.Web.Tests.Unit.Service
{
    using AutoFixture;
    using EA.Weee.Api.Client;
    using EA.Weee.Api.Client.Models.Pay;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Tests.Core;
    using EA.Weee.Web.Services;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public class PaymentServiceTests : SimpleUnitTestBase
    {
        private readonly Fixture fixture;
        private readonly IPayClient payClient;
        private readonly ConfigurationService configurationService;
        private readonly ISecureReturnUrlHelper secureReturnUrlHelper;
        private readonly IPaymentReferenceGenerator paymentReferenceGenerator;
        private readonly IWeeeClient weeeClient;
        private readonly PaymentService paymentService;

        public PaymentServiceTests()
        {
            fixture = new Fixture();
            payClient = A.Fake<IPayClient>();
            configurationService = A.Fake<ConfigurationService>();
            secureReturnUrlHelper = A.Fake<ISecureReturnUrlHelper>();
            paymentReferenceGenerator = A.Fake<IPaymentReferenceGenerator>();
            weeeClient = A.Fake<IWeeeClient>();

            paymentService = new PaymentService(
                payClient,
                configurationService,
                secureReturnUrlHelper,
                paymentReferenceGenerator,
                () => weeeClient);
        }

        [Fact]
        public async Task CreatePaymentAsync_ShouldCreatePaymentAndAddPaymentSession()
        {
            // Arrange
            var directRegistrantId = Guid.NewGuid();
            var email = fixture.Create<string>();
            var accessToken = fixture.Create<string>();
            var secureId = fixture.Create<string>();
            var returnUrl = fixture.Create<string>();
            var paymentReference = fixture.Create<string>();
            var paymentId = fixture.Create<string>();
            var amount = fixture.Create<int>();
            var description = fixture.Create<string>();

            A.CallTo(() => secureReturnUrlHelper.GenerateSecureRandomString(directRegistrantId, 16))
                .Returns(secureId);
            A.CallTo(() => configurationService.CurrentConfiguration.GovUkPayReturnBaseUrl)
                .Returns(returnUrl);
            A.CallTo(() => configurationService.CurrentConfiguration.GovUkPayAmountInPence)
                .Returns(amount);
            A.CallTo(() => configurationService.CurrentConfiguration.GovUkPayDescription)
                .Returns(description);
            A.CallTo(() => paymentReferenceGenerator.GeneratePaymentReferenceWithSeparators(20))
                .Returns(paymentReference);

            var expectedPaymentResult = new CreatePaymentResult { PaymentId = paymentId };
            A.CallTo(() => payClient.CreatePaymentAsync(A<string>._, A<CreateCardPaymentRequest>.That.Matches(c => c.Amount == amount && 
                    c.Description == description &&
                    c.ReturnUrl == returnUrl && 
                    c.Reference == paymentReference)))
                .Returns(expectedPaymentResult);

            // Act
            var result = await paymentService.CreatePaymentAsync(directRegistrantId, email, accessToken);

            // Assert
            result.Should().BeEquivalentTo(expectedPaymentResult);
            A.CallTo(() => weeeClient.SendAsync(accessToken, A<AddPaymentSessionRequest>.That.Matches(a => a.PaymentReturnToken == secureId &&
                    a.Amount == amount &&
                    a.DirectRegistrantId == directRegistrantId &&
                    a.PaymentId == expectedPaymentResult.PaymentId &&
                    a.PaymentReference == paymentReference)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CheckInProgressPaymentAsync_ShouldReturnNullWhenNoPaymentExists()
        {
            // Arrange
            var accessToken = fixture.Create<string>();
            var directRegistrantId = Guid.NewGuid();

            A.CallTo(() => weeeClient.SendAsync(accessToken, A<GetInProgressPaymentSessionRequest>._))
                .Returns<SubmissionPaymentDetails>(null);

            // Act
            var result = await paymentService.CheckInProgressPaymentAsync(accessToken, directRegistrantId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CheckInProgressPaymentAsync_ShouldReturnNullWhenNoGovUkPaymentExists()
        {
            // Arrange
            var accessToken = fixture.Create<string>();
            var directRegistrantId = Guid.NewGuid();
            var paymentId = fixture.Create<string>();
            var paymentSessionId = fixture.Create<Guid>();

            var existingPayment = new SubmissionPaymentDetails
            {
                PaymentId = paymentId,
                PaymentSessionId = paymentSessionId
            };

            A.CallTo(() => weeeClient.SendAsync(accessToken, A<GetInProgressPaymentSessionRequest>._))
                .Returns<SubmissionPaymentDetails>(existingPayment);

            A.CallTo(() => payClient.GetPaymentAsync(paymentId)).Returns<PaymentWithAllLinks>(null);

            // Act
            var result = await paymentService.CheckInProgressPaymentAsync(accessToken, directRegistrantId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CheckInProgressPaymentAsync_ShouldReturnPaymentWhenExists()
        {
            // Arrange
            var accessToken = fixture.Create<string>();
            var directRegistrantId = Guid.NewGuid();
            var paymentId = fixture.Create<string>();
            var paymentSessionId = fixture.Create<Guid>();

            var existingPayment = new SubmissionPaymentDetails
            {
                PaymentId = paymentId,
                PaymentSessionId = paymentSessionId
            };

            A.CallTo(() => weeeClient.SendAsync(accessToken, A<GetInProgressPaymentSessionRequest>._))
                .Returns(existingPayment);

            var expectedPaymentResult = new PaymentWithAllLinks
            {
                PaymentId = paymentId,
                State = new PaymentState { Status = PaymentStatus.Created, Finished = false }
            };

            A.CallTo(() => payClient.GetPaymentAsync(paymentId))
                .Returns(expectedPaymentResult);

            // Act
            var result = await paymentService.CheckInProgressPaymentAsync(accessToken, directRegistrantId);

            // Assert
            result.Should().BeEquivalentTo(expectedPaymentResult);
            A.CallTo(() => weeeClient.SendAsync(accessToken, A<UpdateSubmissionPaymentDetailsRequest>.That.Matches(u =>
                    u.IsFinalState == expectedPaymentResult.State.IsInFinalState() &&
                    u.DirectRegistrantId == directRegistrantId &&
                    u.PaymentSessionId == existingPayment.PaymentSessionId &&
                    u.PaymentStatus == expectedPaymentResult.State.Status)))
                .MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData("https://publicapi.payments.service.gov.uk/some/path", true)]
        [InlineData("https://card.payments.service.gov.uk/some/path", true)]
        [InlineData("https://malicious-site.com", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void ValidateExternalUrl_ShouldReturnExpectedResult(string url, bool expectedResult)
        {
            // Act
            var result = paymentService.ValidateExternalUrl(url);

            // Assert
            result.Should().Be(expectedResult);
        }
        [Fact]
        public async Task CheckInProgressPaymentAsync_ShouldReturnNullForFinishedUnsuccessfulPayment()
        {
            // Arrange
            var accessToken = fixture.Create<string>();
            var directRegistrantId = Guid.NewGuid();
            var paymentId = fixture.Create<string>();
            var paymentSessionId = fixture.Create<Guid>();

            var existingPayment = new SubmissionPaymentDetails
            {
                PaymentId = paymentId,
                PaymentSessionId = paymentSessionId
            };

            A.CallTo(() => weeeClient.SendAsync(accessToken, A<GetInProgressPaymentSessionRequest>._))
                .Returns(existingPayment);

            var unsuccessfulPayment = new PaymentWithAllLinks
            {
                PaymentId = paymentId,
                State = new PaymentState { Status = PaymentStatus.Failed, Finished = true }
            };

            A.CallTo(() => payClient.GetPaymentAsync(paymentId))
                .Returns(unsuccessfulPayment);

            // Act
            var result = await paymentService.CheckInProgressPaymentAsync(accessToken, directRegistrantId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task HandlePaymentReturnAsync_ShouldThrowForErrorMessage()
        {
            // Arrange
            var accessToken = fixture.Create<string>();
            var token = fixture.Create<string>();
            var directRegistrantId = Guid.NewGuid();

            A.CallTo(() => secureReturnUrlHelper.ValidateSecureRandomString(token))
                .Returns((true, directRegistrantId));

            var payment = new SubmissionPaymentDetails
            {
                ErrorMessage = "Some error occurred"
            };

            A.CallTo(() => weeeClient.SendAsync(accessToken, A<ValidateAndGetSubmissionPayment>._))
                .Returns(payment);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                paymentService.HandlePaymentReturnAsync(accessToken, token));
        }

        [Fact]
        public async Task HandlePaymentReturnAsync_ShouldReturnNullForNullPaymentResult()
        {
            // Arrange
            var accessToken = fixture.Create<string>();
            var token = fixture.Create<string>();
            var directRegistrantId = Guid.NewGuid();

            A.CallTo(() => secureReturnUrlHelper.ValidateSecureRandomString(token))
                .Returns((true, directRegistrantId));

            var payment = new SubmissionPaymentDetails
            {
                PaymentId = fixture.Create<string>()
            };

            A.CallTo(() => weeeClient.SendAsync(accessToken, A<ValidateAndGetSubmissionPayment>._))
                .Returns(payment);

            A.CallTo(() => payClient.GetPaymentAsync(payment.PaymentId))
                .Returns<PaymentWithAllLinks>(null);

            // Act
            var result = await paymentService.HandlePaymentReturnAsync(accessToken, token);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task HandlePaymentReturnAsync_ShouldReturnPaymentResultWithStatus()
        {
            // Arrange
            var accessToken = fixture.Create<string>();
            var token = fixture.Create<string>();
            var directRegistrantId = Guid.NewGuid();

            A.CallTo(() => secureReturnUrlHelper.ValidateSecureRandomString(token))
                .Returns((true, directRegistrantId));

            var payment = new SubmissionPaymentDetails
            {
                DirectRegistrantId = directRegistrantId,
                PaymentId = fixture.Create<string>(),
                PaymentSessionId = Guid.NewGuid(),
                PaymentReference = fixture.Create<string>(),
                PaymentFinished = false 
            };

            A.CallTo(() => weeeClient.SendAsync(accessToken, A<ValidateAndGetSubmissionPayment>._))
                .Returns(payment);

            var paymentResult = new PaymentWithAllLinks
            {
                State = new PaymentState { Status = PaymentStatus.Success }
            };

            A.CallTo(() => payClient.GetPaymentAsync(payment.PaymentId))
                .Returns(paymentResult);

            // Act
            var result = await paymentService.HandlePaymentReturnAsync(accessToken, token);

            // Assert
            result.Should().NotBeNull();
            result.DirectRegistrantId.Should().Be(directRegistrantId);
            result.PaymentReference.Should().Be(payment.PaymentReference);
            result.Status.Should().Be(PaymentStatus.Success);

            A.CallTo(() => weeeClient.SendAsync(accessToken,
                A<UpdateSubmissionPaymentDetailsRequest>.That.Matches(r =>
                    r.DirectRegistrantId == payment.DirectRegistrantId &&
                    r.PaymentStatus == paymentResult.State.Status &&
                    r.PaymentSessionId == payment.PaymentSessionId &&
                    r.IsFinalState == paymentResult.State.IsInFinalState())))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandlePaymentReturnAsync_ShouldReturnPaymentResultDirectlyWhenPaymentFinished()
        {
            // Arrange
            var accessToken = fixture.Create<string>();
            var token = fixture.Create<string>();
            var directRegistrantId = Guid.NewGuid();

            A.CallTo(() => secureReturnUrlHelper.ValidateSecureRandomString(token))
                .Returns((true, directRegistrantId));

            var payment = new SubmissionPaymentDetails
            {
                DirectRegistrantId = directRegistrantId,
                PaymentId = fixture.Create<string>(),
                PaymentReference = fixture.Create<string>(),
                PaymentFinished = true,
                PaymentStatus = PaymentStatus.Success
            };

            A.CallTo(() => weeeClient.SendAsync(accessToken, A<ValidateAndGetSubmissionPayment>._))
                .Returns(payment);

            // Act
            var result = await paymentService.HandlePaymentReturnAsync(accessToken, token);

            // Assert
            result.Should().NotBeNull();
            result.DirectRegistrantId.Should().Be(directRegistrantId);
            result.PaymentReference.Should().Be(payment.PaymentReference);
            result.Status.Should().Be(PaymentStatus.Success);

            A.CallTo(() => payClient.GetPaymentAsync(A<string>._))
                .MustNotHaveHappened();
        }
    }
}