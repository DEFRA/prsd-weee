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

            this.httpClient = httpClientFactory.CreateHttpClient(baseUrl, config, logger);
            this.retryPolicy = retryPolicy;
            this.jsonSerializer = jsonSerializer;
            this.tokenProvider = tokenProvider;
            this.logger = logger;
        }

        public async Task<AddressLookupResponse> GetAddressDetailsAsync(string endpoint, string postcode, string buildingNameOrNumber)
        {
            Condition.Requires(endpoint).IsNotNullOrWhiteSpace("Endpoint cannot be null or whitespace.");

            if (!IsValidSearchParameter(postcode, buildingNameOrNumber))
            {
                logger.Warning("Not calling address lookup API invalid search parameters");

                return new AddressLookupResponse()
                {
                    InvalidRequest = true
                };
            }

            try
            {
                var token = await tokenProvider.GetAccessTokenAsync();
                var allResults = new List<AddressResult>();
                var offset = 0;

                // Make initial request to get total result count
                var initialRequestUri = BuildRequestUri(endpoint, postcode);
                var initialResponse = await retryPolicy.ExecuteAsync(() =>
                    httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, initialRequestUri)
                    {
                        Headers = { { "Authorization", $"Bearer {token}" } }
                    })).ConfigureAwait(false);

                initialResponse.EnsureSuccessStatusCode();
                var initialContent = await initialResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                var firstResponse = jsonSerializer.Deserialize<AddressLookupResponse>(initialContent);

                // Ensure firstResponse is properly initialized
                if (firstResponse == null)
                {
                    firstResponse = new AddressLookupResponse
                    {
                        Header = new Header
                        {
                            TotalResults = "0",
                            Query = $"postcode={postcode}",
                            Format = "JSON"
                        },
                        Results = new List<AddressResult>()
                    };
                }

                // Check total results count
                if (!int.TryParse(firstResponse?.Header?.TotalResults, out var totalResults))
                {
                    logger.Warning($"Unable to parse total results from API response for postcode {postcode}");
                    totalResults = 0;
                }

                // If more than 200 results, return response indicating search is too broad as the Defra API only allows a maximum offset of 200
                if (totalResults > 200)
                {
                    logger.Information($"Search for postcode {postcode} returned {totalResults} results which exceeds the maximum of 200. A more specific search is required.");
                    return new AddressLookupResponse()
                    {
                        Header = firstResponse?.Header ?? new Header
                        {
                            TotalResults = totalResults.ToString(),
                            Query = $"postcode={postcode}",
                            Format = "JSON"
                        },
                        Results = new List<AddressResult>(),
                        SearchTooBroad = true
                    };
                }

                // Add first page results
                if (firstResponse.Results != null && firstResponse.Results.Any())
                {
                    allResults.AddRange(firstResponse.Results);
                    logger.Debug($"Retrieved {firstResponse.Results.Count} results from initial page");
                }

                // Get remaining pages if needed
                var pageResponse = firstResponse;
                offset = firstResponse.Results?.Count ?? 0;

                while (offset < totalResults && pageResponse?.Results?.Any() == true)
                {
                    var requestUri = BuildRequestUri(endpoint, postcode, offset);

                    var response = await retryPolicy.ExecuteAsync(() =>
                        httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri)
                        {
                            Headers = { { "Authorization", $"Bearer {token}" } }
                        })).ConfigureAwait(false);

                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    pageResponse = jsonSerializer.Deserialize<AddressLookupResponse>(content);

                    if (pageResponse?.Results != null && pageResponse.Results.Any())
                    {
                        allResults.AddRange(pageResponse.Results);
                        offset += pageResponse.Results.Count;
                    }
                    else
                    {
                        break;
                    }
                }

                // Filter collected results
                if (allResults.Any())
                {
                    var filteredResults = allResults
                        .Where(address =>
                            ((!string.IsNullOrWhiteSpace(address.BuildingNumber) &&
                              address.BuildingNumber.IndexOf(buildingNameOrNumber, StringComparison.OrdinalIgnoreCase) >= 0) ||
                             (!string.IsNullOrWhiteSpace(address.BuildingName) &&
                              address.BuildingName.IndexOf(buildingNameOrNumber, StringComparison.OrdinalIgnoreCase) >= 0) ||
                             (!string.IsNullOrWhiteSpace(address.SubBuildingName) &&
                              address.SubBuildingName.IndexOf(buildingNameOrNumber, StringComparison.OrdinalIgnoreCase) >= 0)))
                        .ToList();

                    var response = new AddressLookupResponse
                    {
                        Header = firstResponse.Header ?? new Header
                        {
                            TotalResults = filteredResults.Count.ToString(),
                            MatchingTotalResults = filteredResults.Count.ToString(),
                            Query = $"postcode={postcode}",
                            Format = "JSON"
                        },
                        Results = filteredResults,
                        Info = firstResponse.Info
                    };

                    // Update the header counts if it exists
                    if (response.Header != null)
                    {
                        response.Header.TotalResults = filteredResults.Count.ToString();
                        response.Header.MatchingTotalResults = filteredResults.Count.ToString();
                    }

                    return response;
                }

                return new AddressLookupResponse
                {
                    Header = firstResponse.Header ?? new Header
                    {
                        TotalResults = "0",
                        Query = $"postcode={postcode}",
                        Format = "JSON"
                    },
                    Results = new List<AddressResult>()
                };
            }
            catch (Exception ex)
            {
                logger.Error($"Error attempting to access address lookup API for {postcode}", ex);
                return new AddressLookupResponse()
                {
                    Error = true,
                    Header = new Header
                    {
                        TotalResults = "0",
                        Query = $"postcode={postcode}",
                        Format = "JSON"
                    },
                    Results = new List<AddressResult>()
                };
            }
        }

        private async Task<AddressLookupResponse> MakeApiRequest(string requestUri, string token)
        {
            try
            {
                var response = await retryPolicy.ExecuteAsync(() =>
                    httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri)
                    {
                        Headers = { { "Authorization", $"Bearer {token}" } }
                    })).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return jsonSerializer.Deserialize<AddressLookupResponse>(content);
            }
            catch (Exception ex)
            {
                logger.Error($"Error making API request to {requestUri}", ex);
                return null;
            }
        }

        private List<AddressResult> FilterResults(List<AddressResult> results, string buildingNameOrNumber)
        {
            if (results == null || !results.Any())
            {
                return new List<AddressResult>();
            }

            return results
                .Where(address =>
                    ((!string.IsNullOrWhiteSpace(address.BuildingNumber) &&
                      address.BuildingNumber.IndexOf(buildingNameOrNumber, StringComparison.OrdinalIgnoreCase) >= 0) ||
                     (!string.IsNullOrWhiteSpace(address.BuildingName) &&
                      address.BuildingName.IndexOf(buildingNameOrNumber, StringComparison.OrdinalIgnoreCase) >= 0) ||
                     (!string.IsNullOrWhiteSpace(address.SubBuildingName) &&
                      address.SubBuildingName.IndexOf(buildingNameOrNumber, StringComparison.OrdinalIgnoreCase) >= 0)))
                .ToList();
        }

        private static bool IsValidSearchParameter(string postcode, string buildingNameOrNumber)
        {
            if (string.IsNullOrWhiteSpace(postcode) || string.IsNullOrWhiteSpace(buildingNameOrNumber))
            {
                return false;
            }

            postcode = postcode.Replace(" ", string.Empty).ToUpper();

            var isValidPostcode = ExternalAddressValidator.UkPostcodeRegex.IsMatch(postcode);

            if (!isValidPostcode)
            {
                return false;
            }

            return true;
        }

        private string BuildRequestUri(string endpoint, string postcode, int? offset = null)
        {
            // Remove any trailing '?' or '&' from the endpoint
            endpoint = endpoint.TrimEnd('?', '&');

            // Construct the base URI
            var requestUri = endpoint;

            // Add postcode parameter
            if (!endpoint.Contains("?postcode=") && !endpoint.Contains("&postcode="))
            {
                requestUri += (endpoint.Contains("?") ? "&" : "?") + $"postcode={postcode}";
            }
            else
            {
                requestUri += postcode;
            }

            // Add offset if provided
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