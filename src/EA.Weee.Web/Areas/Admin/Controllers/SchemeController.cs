namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Base;
    using Infrastructure;
    using ViewModels;
    using Weee.Requests.PCS;

    public class SchemeController : AdminController
    {
        private readonly Func<IWeeeClient> apiClient;

        public SchemeController(Func<IWeeeClient> apiClient)
        {
            this.apiClient = apiClient;
        }

        [HttpGet]
        public async Task<ViewResult> ManageSchemes()
        {
            using (var client = apiClient())
            {
                var schemes = await client.SendAsync(User.GetAccessToken(), new GetPcses());
                return View(new ManageSchemesViewModel{ Schemes = schemes });
            }
        }

        [HttpPost]
        public async Task<ActionResult> ManageSchemes(ManageSchemesViewModel viewModel)
        {
            // verify here that the user is allowed to look at the supplied PCS?

            return RedirectToAction("ManageScheme", new { schemeId = viewModel.Selected });
        }

        [HttpGet]
        public async Task<ViewResult> ManageScheme(Guid schemeId)
        {
            throw new NotImplementedException();
        }
    }
}