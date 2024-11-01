namespace EA.Weee.Api.Tests.Unit.Client
{
    using EA.Weee.Api.Client;
    using EA.Weee.Api.Client.Models.AddressLookup;
    using EA.Weee.Api.Client.Serlializer;
    using FakeItEasy;
    using FluentAssertions;
    using Serilog;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class AddressLookupClientTests
    {
        private readonly IHttpClientWrapper httpClient;
        private readonly IRetryPolicyWrapper retryPolicy;
        private readonly IJsonSerializer jsonSerializer;
        private readonly ILogger logger;
        private readonly IOAuthTokenProvider tokenProvider;
        private readonly AddressLookupClient addressLookupClient;

        public AddressLookupClientTests()
        {
            httpClient = A.Fake<IHttpClientWrapper>();
            retryPolicy = A.Fake<IRetryPolicyWrapper>();
            jsonSerializer = A.Fake<IJsonSerializer>();
            logger = A.Fake<ILogger>();
            tokenProvider = A.Fake<IOAuthTokenProvider>();

            var httpClientFactory = A.Fake<IHttpClientWrapperFactory>();
            A.CallTo(() => httpClientFactory.CreateHttpClient(
                A<string>._, A<HttpClientHandlerConfig>._, A<ILogger>._))
                .Returns(httpClient);

            addressLookupClient = new AddressLookupClient(
                "http://defra.com",
                httpClientFactory,
                retryPolicy,
                jsonSerializer,
                new HttpClientHandlerConfig(),
                logger,
                tokenProvider);
        }

        [Theory]
        [InlineData("SW1A 1AA")]
        [InlineData("SW1A1AA")]
        public async Task GetAddressesAsync_WithValidPostcode_ShouldReturnAddresses(string postcode)
        {
            // Arrange
            const string endpoint = "api/addresses";
            const string token = "token";
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("{}")
            };

            var expectedResult = new AddressLookupResponse
            {
                Header = new Header { TotalResults = "1" },
                Results = new List<AddressResult> { new AddressResult { Postcode = postcode } }
            };

            A.CallTo(() => tokenProvider.GetAccessTokenAsync()).Returns(token);
            A.CallTo(() => httpClient.SendAsync(A<HttpRequestMessage>._, A<CancellationToken>._))
                .Returns(response);

            A.CallTo(() => retryPolicy.ExecuteAsync(A<Func<Task<HttpResponseMessage>>>._))
                .ReturnsLazily(call =>
                {
                    var func = call.GetArgument<Func<Task<HttpResponseMessage>>>(0);
                    return func();
                });

            A.CallTo(() => jsonSerializer.Deserialize<AddressLookupResponse>(A<string>._))
                .Returns(expectedResult);

            // Act
            var result = await addressLookupClient.GetAddressesAsync(endpoint, postcode);

            // Assert
            result.Should().NotBeNull();
            result.Results.Should().HaveCount(1);
            result.Error.Should().BeFalse();
            result.InvalidRequest.Should().BeFalse();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [InlineData("NOT A POSTCODE")]
        public async Task GetAddressesAsync_WithInvalidPostcode_ShouldReturnInvalidRequest(string postcode)
        {
            // Arrange
            const string endpoint = "api/addresses";

            // Act
            var result = await addressLookupClient.GetAddressesAsync(endpoint, postcode);

            // Assert
            result.Should().NotBeNull();
            result.InvalidRequest.Should().BeTrue();
            result.Error.Should().BeFalse();
            A.CallTo(() => httpClient.SendAsync(A<HttpRequestMessage>._, A<CancellationToken>._))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task GetAddressesAsync_WhenTooManyResults_ShouldReturnSearchTooBroad()
        {
            // Arrange
            const string endpoint = "api/addresses";
            const string postcode = "SW1A 1AA";
            const string token = "token";
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("{}")
            };

            var initialResponse = new AddressLookupResponse
            {
                Header = new Header { TotalResults = "201" },
                Results = new List<AddressResult>()
            };

            A.CallTo(() => tokenProvider.GetAccessTokenAsync()).Returns(token);
            A.CallTo(() => httpClient.SendAsync(A<HttpRequestMessage>._, A<CancellationToken>._))
                .Returns(response);

            A.CallTo(() => retryPolicy.ExecuteAsync(A<Func<Task<HttpResponseMessage>>>._))
                .ReturnsLazily(call =>
                {
                    var func = call.GetArgument<Func<Task<HttpResponseMessage>>>(0);
                    return func();
                });

            A.CallTo(() => jsonSerializer.Deserialize<AddressLookupResponse>(A<string>._))
                .Returns(initialResponse);

            // Act
            var result = await addressLookupClient.GetAddressesAsync(endpoint, postcode);

            // Assert
            result.Should().NotBeNull();
            result.SearchTooBroad.Should().BeTrue();
            result.Results.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAddressDetailsAsync_WithValidBuildingSearch_ShouldReturnFilteredResults()
        {
            // Arrange
            const string endpoint = "api/addresses";
            const string postcode = "SW1A 1AA";
            const string buildingNumber = "10";
            const string token = "token";
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("{}")
            };

            var initialResponse = new AddressLookupResponse
            {
                Header = new Header { TotalResults = "2" },
                Results = new List<AddressResult>
                {
                    new AddressResult { BuildingNumber = "10", Postcode = postcode },
                    new AddressResult { BuildingNumber = "11", Postcode = postcode }
                }
            };

            A.CallTo(() => tokenProvider.GetAccessTokenAsync()).Returns(token);
            A.CallTo(() => httpClient.SendAsync(A<HttpRequestMessage>._, A<CancellationToken>._))
                .Returns(response);

            A.CallTo(() => retryPolicy.ExecuteAsync(A<Func<Task<HttpResponseMessage>>>._))
                .ReturnsLazily(call =>
                {
                    var func = call.GetArgument<Func<Task<HttpResponseMessage>>>(0);
                    return func();
                });

            A.CallTo(() => jsonSerializer.Deserialize<AddressLookupResponse>(A<string>._))
                .ReturnsNextFromSequence(initialResponse, initialResponse);

            // Act
            var result = await addressLookupClient.GetAddressDetailsAsync(endpoint, postcode, buildingNumber);

            // Assert
            result.Should().NotBeNull();
            result.Results.Should().HaveCount(1);
            result.Results.First().BuildingNumber.Should().Be(buildingNumber);
        }

        [Fact]
        public async Task GetAddressDetailsAsync_WhenApiThrowsException_ShouldReturnError()
        {
            // Arrange
            const string endpoint = "api/addresses";
            const string postcode = "SW1A 1AA";
            const string buildingNumber = "10";
            var exception = new Exception("API Error");

            A.CallTo(() => tokenProvider.GetAccessTokenAsync())
                .ThrowsAsync(exception);

            // Act
            var result = await addressLookupClient.GetAddressDetailsAsync(endpoint, postcode, buildingNumber);

            // Assert
            result.Should().NotBeNull();
            result.Error.Should().BeTrue();
            A.CallTo(() => logger.Error(
                A<Exception>.That.IsInstanceOf(typeof(Exception)),
                A<string>.That.IsEqualTo("Error attempting to access address lookup API for {Postcode}"),
                A<string>.That.IsEqualTo(postcode)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Dispose_ShouldDisposeHttpClientOnce()
        {
            // Act
            addressLookupClient.Dispose();
            addressLookupClient.Dispose(); // Second dispose should be ignored

            // Assert
            A.CallTo(() => httpClient.Dispose()).MustHaveHappenedOnceExactly();
        }
    }
}