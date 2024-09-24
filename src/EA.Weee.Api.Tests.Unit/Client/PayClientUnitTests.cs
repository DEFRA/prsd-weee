namespace EA.Weee.Api.Tests.Unit.Client
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using EA.Weee.Api.Client;
    using EA.Weee.Api.Client.Models.Pay;
    using EA.Weee.Api.Client.Serlializer;
    using FakeItEasy;
    using FluentAssertions;
    using Serilog;
    using Xunit;

    public class PayClientTests
    {
        private readonly IHttpClientWrapper httpClient;
        private readonly IRetryPolicyWrapper retryPolicy;
        private readonly IJsonSerializer jsonSerialiser;
        private readonly ILogger logger;
        private readonly PayClient payClient;

        public PayClientTests()
        {
            httpClient = A.Fake<IHttpClientWrapper>();
            retryPolicy = A.Fake<IRetryPolicyWrapper>();
            jsonSerialiser = A.Fake<IJsonSerializer>();
            logger = A.Fake<ILogger>();

            var fakeHttpClientFactory = A.Fake<IHttpClientWrapperFactory>();
            A.CallTo(() => fakeHttpClientFactory.CreateHttpClientWithAuthorization(
                A<string>._, A<HttpClientHandlerConfig>._, A<ILogger>._, A<string>._, A<string>._))
                .Returns(httpClient);

            payClient = new PayClient(
                "http://example.com",
                "fake-api-key",
                fakeHttpClientFactory,
                retryPolicy,
                jsonSerialiser,
                new HttpClientHandlerConfig(),
                logger);
        }

        [Fact]
        public async Task CreatePaymentAsync_ShouldCallHttpClientPostAsync()
        {
            // Arrange
            var fakeHttpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("{}")
            };

            A.CallTo(() => httpClient.PostAsync(A<string>._, A<HttpContent>._, A<System.Threading.CancellationToken>._))
                .Returns(fakeHttpResponseMessage);

            A.CallTo(() => retryPolicy.ExecuteAsync(A<Func<Task<HttpResponseMessage>>>._))
                .ReturnsLazily(call =>
                {
                    var func = call.GetArgument<Func<Task<HttpResponseMessage>>>(0);
                    return func();
                });

            var request = new CreateCardPaymentRequest();
            var expectedResult = new CreatePaymentResult();
            A.CallTo(() => jsonSerialiser.Serialize(request)).Returns("serialized-request");
            A.CallTo(() => jsonSerialiser.Deserialize<CreatePaymentResult>(A<string>._))
                .Returns(expectedResult);

            // Act
            var result = await payClient.CreatePaymentAsync("idempotency-key", request);

            // Assert
            A.CallTo(() => httpClient.PostAsync(A<string>.That.Contains("v1/payments"), A<HttpContent>._, A<System.Threading.CancellationToken>._))
                .MustHaveHappenedOnceExactly();
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetPaymentAsync_ShouldCallHttpClientGetAsync()
        {
            // Arrange
            var fakeHttpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("{}")
            };

            A.CallTo(() => httpClient.GetAsync(A<string>._)).Returns(fakeHttpResponseMessage);

            A.CallTo(() => retryPolicy.ExecuteAsync(A<Func<Task<HttpResponseMessage>>>._))
                .ReturnsLazily(call =>
                {
                    var func = call.GetArgument<Func<Task<HttpResponseMessage>>>(0);
                    return func();
                });

            var expectedResult = new PaymentWithAllLinks();
            A.CallTo(() => jsonSerialiser.Deserialize<PaymentWithAllLinks>(A<string>._))
                .Returns(expectedResult);

            // Act
            var result = await payClient.GetPaymentAsync("payment-id");

            // Assert
            A.CallTo(() => httpClient.GetAsync(A<string>.That.Contains("v1/payments/payment-id")))
                .MustHaveHappenedOnceExactly();
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void Dispose_ShouldDisposeHttpClient()
        {
            // Act
            payClient.Dispose();

            // Assert
            A.CallTo(() => httpClient.Dispose()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CreatePaymentAsync_ShouldHandleHttpRequestException()
        {
            // Arrange
            A.CallTo(() => httpClient.PostAsync(A<string>._, A<HttpContent>._, A<System.Threading.CancellationToken>._))
                .Throws(new HttpRequestException("Simulated network error"));

            A.CallTo(() => retryPolicy.ExecuteAsync(A<Func<Task<HttpResponseMessage>>>._))
                .ReturnsLazily(call =>
                {
                    var func = call.GetArgument<Func<Task<HttpResponseMessage>>>(0);
                    return func();
                });

            var request = new CreateCardPaymentRequest();

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                payClient.CreatePaymentAsync("idempotency-key", request));

            A.CallTo(() => logger.Error(A<Exception>._, A<string>._)).MustHaveHappenedOnceExactly();
        }
    }
}