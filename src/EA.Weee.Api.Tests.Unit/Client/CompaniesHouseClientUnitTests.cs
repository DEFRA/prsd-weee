namespace EA.Weee.Api.Tests.Unit.Client
{
    using System;
    using System.Net.Http;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using EA.Weee.Api.Client;
    using EA.Weee.Api.Client.Models;
    using EA.Weee.Api.Client.Serlializer;
    using FakeItEasy;
    using FluentAssertions;
    using Serilog;
    using Xunit;

    public class CompaniesHouseClientTests
    {
        private readonly IHttpClientWrapper httpClient;
        private readonly IRetryPolicyWrapper retryPolicy;
        private readonly IJsonSerializer jsonSerializer;
        private readonly ILogger logger;
        private readonly CompaniesHouseClient companiesHouseClient;

        public CompaniesHouseClientTests()
        {
            httpClient = A.Fake<IHttpClientWrapper>();
            retryPolicy = A.Fake<IRetryPolicyWrapper>();
            jsonSerializer = A.Fake<IJsonSerializer>();
            logger = A.Fake<ILogger>();

            var fakeHttpClientFactory = A.Fake<IHttpClientWrapperFactory>();
            A.CallTo(() => fakeHttpClientFactory.CreateHttpClientWithCertificate(
                A<string>._, A<HttpClientHandlerConfig>._, A<ILogger>._, A<X509Certificate2>._))
                .Returns(httpClient);

            companiesHouseClient = new CompaniesHouseClient(
                "http://example.com",
                fakeHttpClientFactory,
                retryPolicy,
                jsonSerializer,
                new HttpClientHandlerConfig(),
                new X509Certificate2(),
                logger);
        }

        [Fact]
        public async Task GetCompanyDetailsAsync_WithValidCompanyReference_ShouldReturnCompanyDetails()
        {
            // Arrange
            const string endpoint = "api/companies";
            const string companyReference = "12345678";
            var fakeHttpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("{}")
            };

            A.CallTo(() => httpClient.GetAsync(A<string>._))
                .Returns(fakeHttpResponseMessage);

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
            A.CallTo(() => httpClient.GetAsync(A<string>.That.Contains($"{endpoint}/{companyReference}")))
                .MustHaveHappenedOnceExactly();
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetCompanyDetailsAsync_WithInvalidCompanyReference_ShouldReturnNull()
        {
            // Arrange
            const string endpoint = "api/companies";
            const string companyReference = "invalid123";

            // Act
            var result = await companiesHouseClient.GetCompanyDetailsAsync(endpoint, companyReference);

            // Assert
            result.Should().BeNull();
            A.CallTo(() => httpClient.GetAsync(A<string>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task GetCompanyDetailsAsync_WhenApiReturnsBadRequest_ShouldReturnNull()
        {
            // Arrange
            var endpoint = "api/companies";
            var companyReference = "12345678";
            var fakeHttpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);

            A.CallTo(() => httpClient.GetAsync(A<string>._))
                .Returns(fakeHttpResponseMessage);

            A.CallTo(() => retryPolicy.ExecuteAsync(A<Func<Task<HttpResponseMessage>>>._))
                .ReturnsLazily(call =>
                {
                    var func = call.GetArgument<Func<Task<HttpResponseMessage>>>(0);
                    return func();
                });

            // Act
            var result = await companiesHouseClient.GetCompanyDetailsAsync(endpoint, companyReference);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void Dispose_ShouldSetDisposedToTrue()
        {
            // Act
            companiesHouseClient.Dispose();

            companiesHouseClient.Invoking(c => c.Dispose()).Should().NotThrow();
        }
    }
}