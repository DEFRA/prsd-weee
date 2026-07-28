namespace EA.Weee.Web.Controllers
{
    using EA.Prsd.Core.Web.OAuth;
    using EA.Weee.Api.Client;
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class BannerController : Controller
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly Func<IOAuthClientCredentialClient> apiClientCredential;

        public BannerController(Func<IWeeeClient> apiClient, Func<IOAuthClientCredentialClient> apiClientCredential)
        {
            this.apiClient = apiClient;
            this.apiClientCredential = apiClientCredential;
        }

        // GET: Banner
        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> MessageBannerAsync()
        {
            using (var client = apiClient())
            {
                var access = await apiClientCredential().GetClientCredentialsAsync();

                var messageBannerData = await client.User.GetMessageBannerAsync(access.AccessToken);

                return Json(messageBannerData, JsonRequestBehavior.AllowGet);
            }
        }
    }
}