namespace EA.Weee.Api.Client
{
    using CuttingEdge.Conditions;
    using EA.Weee.Api.Client.Models.AddressLookup;
    using EA.Weee.Api.Client.Serlializer;
    using EA.Weee.Core.Validation;
    using Serilog;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class AddressLookupClient : IAddressLookupClient
    {
        private const int MaxResultsLimit = 200;
        private readonly IRetryPolicyWrapper retryPolicy;
        private readonly IJsonSerializer jsonSerializer;
        private readonly IHttpClientWrapper httpClient;
        private readonly IOAuthTokenProvider tokenProvider;
        private readonly ILogger logger;
        private bool disposed;

        public AddressLookupClient(
            string baseUrl,
            IHttpClientWrapperFactory httpClientFactory,
            IRetryPolicyWrapper retryPolicy,
            IJsonSerializer jsonSerializer,
            HttpClientHandlerConfig config,
            ILogger logger,
            IOAuthTokenProvider tokenProvider)
        {
            Condition.Requires(baseUrl).IsNotNullOrWhiteSpace();
            Condition.Requires(httpClientFactory).IsNotNull();
            Condition.Requires(retryPolicy).IsNotNull();
            Condition.Requires(jsonSerializer).IsNotNull();
            Condition.Requires(config).IsNotNull();
            Condition.Requires(logger).IsNotNull();
            Condition.Requires(tokenProvider).IsNotNull();

            httpClient = httpClientFactory.CreateHttpClient(baseUrl, config, logger);
            this.retryPolicy = retryPolicy;
            this.jsonSerializer = jsonSerializer;
            this.tokenProvider = tokenProvider;
            this.logger = logger;
        }

        public async Task<AddressLookupResponse> GetAddressesAsync(string endpoint, string postcode)
        {
            Condition.Requires(endpoint).IsNotNullOrWhiteSpace("Endpoint cannot be null or whitespace.");

            if (!IsValidPostcode(postcode))
            {
                logger.Warning("Invalid postcode format provided: {Postcode}", postcode);
                return CreateErrorResponse(postcode, invalidRequest: true);
            }

            try
            {
                var token = await tokenProvider.GetAccessTokenAsync();
                var initialResponse = await GetPagedResultsAsync(endpoint, postcode, token, 0);

                if (initialResponse.Error)
                {
                    return initialResponse;
                }

                var totalResults = GetTotalResults(initialResponse);

                if (totalResults > MaxResultsLimit)
                {
                    return CreateTooManyResultsResponse(postcode, totalResults, initialResponse.Header);
                }

                return await FetchAllResults(endpoint, postcode, token, initialResponse, totalResults);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error attempting to access address lookup API for {Postcode}", postcode);
                return CreateErrorResponse(postcode);
            }
        }

        public async Task<AddressLookupResponse> GetAddressDetailsAsync(string endpoint, string postcode, string buildingNameOrNumber)
        {
            Condition.Requires(endpoint).IsNotNullOrWhiteSpace("Endpoint cannot be null or whitespace.");

            if (!IsValidSearchParameter(postcode, buildingNameOrNumber))
            {
                logger.Warning("Not calling address lookup API invalid search parameters");
                return CreateErrorResponse(postcode, invalidRequest: true);
            }

            try
            {
                var token = await tokenProvider.GetAccessTokenAsync();
                var initialResponse = await GetPagedResultsAsync(endpoint, postcode, token, 0);
                var totalResults = GetTotalResults(initialResponse);

                if (totalResults > MaxResultsLimit)
                {
                    return CreateTooManyResultsResponse(postcode, totalResults, initialResponse.Header);
                }

                var allResults = await FetchAllResults(endpoint, postcode, token, initialResponse, totalResults);
                return FilterResults(allResults, buildingNameOrNumber, postcode);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error attempting to access address lookup API for {Postcode}", postcode);
                return CreateErrorResponse(postcode);
            }
        }

        private async Task<AddressLookupResponse> GetPagedResultsAsync(string endpoint, string postcode, string token, int offset)
        {
            var requestUri = BuildRequestUri(endpoint, postcode, offset);

            try
            {
                var response = await retryPolicy.ExecuteAsync(async () =>
                {
                    using (var request = CreateHttpRequest(requestUri, token))
                    {
                        return await httpClient.SendAsync(request).ConfigureAwait(false);
                    }
                }).ConfigureAwait(false);

                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return CreateEmptyResponse(postcode);
                }

                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return jsonSerializer.Deserialize<AddressLookupResponse>(content) ?? CreateEmptyResponse(postcode);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to get paged results for postcode {Postcode}", postcode);
                return CreateErrorResponse(postcode);
            }
        }

        private async Task<AddressLookupResponse> FetchAllResults(string endpoint, string postcode, string token,
            AddressLookupResponse initialResponse, int totalResults)
        {
            var allResults = new List<AddressResult>();
            if (initialResponse.Results?.Any() == true)
            {
                allResults.AddRange(initialResponse.Results);
            }

            var offset = allResults.Count;
            while (offset < totalResults)
            {
                var pageResponse = await GetPagedResultsAsync(endpoint, postcode, token, offset);

                if (pageResponse.Error || pageResponse.Results?.Any() != true)
                {
                    break;
                }

                allResults.AddRange(pageResponse.Results);
                offset += pageResponse.Results.Count;
            }

            return new AddressLookupResponse
            {
                Header = initialResponse.Header ?? CreateDefaultHeader(postcode, allResults.Count),
                Results = allResults,
                Info = initialResponse.Info
            };
        }

        private static AddressLookupResponse FilterResults(AddressLookupResponse response, string buildingNameOrNumber, string postcode)
        {
            var filteredResults = response.Results?.Where(address => MatchesBuildingIdentifier(address, buildingNameOrNumber))
                .ToList() ?? new List<AddressResult>();

            var header = response.Header ?? CreateDefaultHeader(postcode, filteredResults.Count);
            header.TotalResults = filteredResults.Count.ToString();
            header.MatchingTotalResults = filteredResults.Count.ToString();

            return new AddressLookupResponse
            {
                Header = header,
                Results = filteredResults,
                Info = response.Info
            };
        }

        private static bool MatchesBuildingIdentifier(AddressResult address, string buildingNameOrNumber)
        {
            return ContainsIgnoreCase(address.BuildingNumber, buildingNameOrNumber) ||
                   ContainsIgnoreCase(address.BuildingName, buildingNameOrNumber) ||
                   ContainsIgnoreCase(address.SubBuildingName, buildingNameOrNumber);
        }

        private static bool ContainsIgnoreCase(string source, string target)
        {
            return !string.IsNullOrWhiteSpace(source) &&
                   source.IndexOf(target, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static HttpRequestMessage CreateHttpRequest(string requestUri, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.Add("Authorization", $"Bearer {token}");
            return request;
        }

        private static bool IsValidPostcode(string postcode)
        {
            if (string.IsNullOrWhiteSpace(postcode))
            {
                return false;
            }

            postcode = postcode.Replace(" ", string.Empty).ToUpper();
            return ExternalAddressValidator.IsValidPartialPostcode(postcode);
        }

        private static bool IsValidSearchParameter(string postcode, string buildingNameOrNumber)
        {
            return !string.IsNullOrWhiteSpace(buildingNameOrNumber) && IsValidPostcode(postcode);
        }

        private static int GetTotalResults(AddressLookupResponse response)
        {
            return int.TryParse(response?.Header?.TotalResults, out var total) ? total : 0;
        }

        private static Header CreateDefaultHeader(string postcode, int totalResults) => new Header()
        {
            TotalResults = totalResults.ToString(),
            Query = $"postcode={postcode}",
            Format = "JSON"
        };

        private static AddressLookupResponse CreateEmptyResponse(string postcode) => new AddressLookupResponse() 
        {
                Header = CreateDefaultHeader(postcode, 0),
                Results = new List<AddressResult>()
        };

        private static AddressLookupResponse CreateErrorResponse(string postcode, bool invalidRequest = false) => new AddressLookupResponse() 
        {
                Error = !invalidRequest,
                InvalidRequest = invalidRequest,
                Header = CreateDefaultHeader(postcode, 0),
                Results = new List<AddressResult>()
        };

        private static AddressLookupResponse CreateTooManyResultsResponse(string postcode, int totalResults, Header existingHeader) =>
            new AddressLookupResponse()
            {
                Header = existingHeader ?? CreateDefaultHeader(postcode, totalResults),
                Results = new List<AddressResult>(),
                SearchTooBroad = true
            };

        private static string BuildRequestUri(string endpoint, string postcode, int? offset = null)
        {
            endpoint = endpoint.TrimEnd('?', '&');
            var requestUri = endpoint;

            if (!endpoint.Contains("?postcode=") && !endpoint.Contains("&postcode="))
            {
                requestUri += (endpoint.Contains("?") ? "&" : "?") + $"postcode={postcode}";
            }
            else
            {
                requestUri += postcode;
            }

            if (offset > 0)
            {
                requestUri += $"&offset={offset.Value}";
            }

            return requestUri;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }
            if (disposing)
            {
                httpClient?.Dispose();
            }
            disposed = true;
        }
    }
}