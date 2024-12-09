namespace EA.Weee.Api.Tests.Unit.Client
{
    using EA.Weee.Api.Client;
    using EA.Weee.Api.Client.Models;
    using EA.Weee.Api.Client.Serlializer;
    using FakeItEasy;
    using FluentAssertions;
    using Serilog;
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class CompaniesHouseClientTests
    {
        private readonly IHttpClientWrapper httpClient;
        private readonly IRetryPolicyWrapper retryPolicy;
        private readonly IJsonSerializer jsonSerializer;
        private readonly ILogger logger;
        private readonly IOAuthTokenProvider tokenProvider;
        private readonly CompaniesHouseClient companiesHouseClient;

        public CompaniesHouseClientTests()
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

            companiesHouseClient = new CompaniesHouseClient(
                "http://example.com",
                httpClientFactory,
                retryPolicy,
                jsonSerializer,
                new HttpClientHandlerConfig(),
                logger,
                tokenProvider);
        }

        [Fact]
        public async Task GetCompanyDetailsAsync_WithValidCompanyReference_ShouldReturnCompanyDetails()
        {
            // Arrange
            const string endpoint = "api/companies";
            const string companyReference = "12345678";
            const string token = "token";
            var httpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("{}")
            };

            A.CallTo(() => tokenProvider.GetAccessTokenAsync()).Returns(token);
            A.CallTo(() => httpClient.SendAsync(A<HttpRequestMessage>._, A<CancellationToken>._))
                .Returns(httpResponseMessage);

            A.CallTo(() => retryPolicy.ExecuteAsync(A<Func<Task<HttpResponseMessage>>>._))
                .ReturnsLazily(call =>
                {
                    var func = call.GetArgument<Func<Task<HttpResponseMessage>>>(0);
                    return func();
                });

            var expectedResult = new DefraCompaniesHouseApiModel();
            A.CallTo(() => jsonSerializer.Deserialize<DefraCompaniesHouseApiModel>(A<string>._))
                .Returns(expectedResult);

            // Act
            var result = await companiesHouseClient.GetCompanyDetailsAsync(endpoint, companyReference);

            // Assert
            A.CallTo(() => httpClient.SendAsync(A<HttpRequestMessage>.That.Matches(
                    r => r.RequestUri.ToString().Contains($"{endpoint}/{companyReference}") &&
                         r.Headers.Authorization.Scheme == "Bearer" &&
                         r.Headers.Authorization.Parameter == token), A<CancellationToken>._))
                .MustHaveHappenedOnceExactly();
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetCompanyDetailsAsync_WithInvalidCompanyReference_ShouldReturnError()
        {
            // Arrange
            const string endpoint = "api/companies";
            const string companyReference = " ";

            // Act
            var result = await companiesHouseClient.GetCompanyDetailsAsync(endpoint, companyReference);

            // Assert
            result.InvalidReference.Should().BeTrue();
            result.HasError.Should().BeTrue();
            result.Error.Should().BeFalse();
            A.CallTo(() => httpClient.SendAsync(A<HttpRequestMessage>._, A<CancellationToken>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task GetCompanyDetailsAsync_WhenApiReturnsBadRequest_ShouldReturnHasError()
        {
            // Arrange
            const string endpoint = "api/companies";
            const string companyReference = "12345678";
            var httpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);

            A.CallTo(() => tokenProvider.GetAccessTokenAsync()).Returns("token");
            A.CallTo(() => httpClient.SendAsync(A<HttpRequestMessage>._, A<CancellationToken>._))
                .Returns(httpResponseMessage);

            A.CallTo(() => retryPolicy.ExecuteAsync(A<Func<Task<HttpResponseMessage>>>._))
                .ReturnsLazily(call =>
                {
                    var func = call.GetArgument<Func<Task<HttpResponseMessage>>>(0);
                    return func();
                });

            // Act
            var result = await companiesHouseClient.GetCompanyDetailsAsync(endpoint, companyReference);

            // Assert
            result.InvalidReference.Should().BeFalse();
            result.HasError.Should().BeTrue();
            result.Error.Should().BeTrue();
        }

        [Fact]
        public void Dispose_ShouldSetDisposedToTrue()
        {
            // Act
            companiesHouseClient.Dispose();

            // Assert
            companiesHouseClient.Invoking(c => c.Dispose()).Should().NotThrow();
        }

        [Fact]
        public async Task GetCompanyDetailsAsync_WithInvalidCompanyReference_ShouldReturnModelWithInvalidReferenceTrue()
        {
            // Arrange
            const string endpoint = "api/companies";
            const string companyReference = " ";

            // Act
            var result = await companiesHouseClient.GetCompanyDetailsAsync(endpoint, companyReference);

            // Assert
            result.Should().NotBeNull();
            result.InvalidReference.Should().BeTrue();
            A.CallTo(() => logger.Warning("Not calling companies house API invalid reference")).MustHaveHappenedOnceExactly();
            A.CallTo(() => httpClient.SendAsync(A<HttpRequestMessage>._, A<CancellationToken>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task GetCompanyDetailsAsync_WhenApiThrowsBadRequestException_ShouldReturnModelWithErrorTrue()
        {
            // Arrange
            const string endpoint = "api/companies";
            const string companyReference = "12345678";
            var badRequestException = new Exception("Companies house bad request");

            A.CallTo(() => tokenProvider.GetAccessTokenAsync()).Returns("token");
            A.CallTo(() => retryPolicy.ExecuteAsync(A<Func<Task<HttpResponseMessage>>>._))
                .ThrowsAsync(badRequestException);

            // Act
            var result = await companiesHouseClient.GetCompanyDetailsAsync(endpoint, companyReference);

            // Assert
            result.Should().NotBeNull();
            result.Error.Should().BeTrue();
            A.CallTo(() => logger.Error(A<string>.That.Contains($"Error attempting to access companies house API for {companyReference}"),
                badRequestException)).MustHaveHappenedOnceExactly();
        }
    }
}