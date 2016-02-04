namespace EA.Weee.Api.Controllers
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Prsd.Core.Web.OAuth;
    using Prsd.Core.Web.OpenId;

    /// <summary>
    /// This controller provides an action which warms up the Thinktecure IdentityServer
    /// by loading the user info for a "warm up user". This action should be called each
    /// time the application restarts to minimise the impact on the first real user.
    /// </summary>
    public class WarmUpController : ApiController
    {
        [AllowAnonymous]
        [HttpGet]
        [Route("Warmup")]
        public async Task<string> Index()
        {
            try
            {
                string username = ConfigurationManager.AppSettings["Weee.WarmUpUserUsername"];
                string password = ConfigurationManager.AppSettings["Weee.WarmUpUserPassword"];

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    return "Skipping warm-up - No user details provided.";
                }

                Stopwatch stopwatch = Stopwatch.StartNew();

                string baseUrl = ConfigurationManager.AppSettings["Weee.SiteRoot"];
                string clientId = ConfigurationManager.AppSettings["Weee.ApiClientID"];
                string clientSecret = ConfigurationManager.AppSettings["Weee.ApiSecret"];

                // Fetch an access token for the warm-up user.
                IOAuthClient client = new OAuthClient(baseUrl, clientId, clientSecret);
                var tokenResponse = await client.GetAccessTokenAsync(username, password);

                if (tokenResponse.AccessToken == null)
                {
                    if (tokenResponse.IsHttpError)
                    {
                        throw new Exception(string.Format("Request for access token returned HttpError: Status - {0}, Reason - {1}, Error - {2}",
                            tokenResponse.HttpErrorStatusCode, tokenResponse.HttpErrorReason, tokenResponse.Error));
                    }
                    else if (tokenResponse.IsError)
                    {
                        throw new Exception(string.Format("Request for access token returned Error: {0}", tokenResponse.Error));
                    }
                    else
                    {
                        throw new Exception("Request for access token returned null.");
                    }
                }

                // Fetch the user info for the warm-up user.
                IUserInfoClient userInfoClient = new UserInfoClient(baseUrl);
                var userInfo = await userInfoClient.GetUserInfoAsync(tokenResponse.AccessToken);

                return string.Format("Warm-up complete in {0} seconds.", stopwatch.Elapsed.TotalSeconds);
            }
            catch (Exception ex)
            {
                throw new Exception("API warm-up failed.", ex);
            }
        }
    }
}