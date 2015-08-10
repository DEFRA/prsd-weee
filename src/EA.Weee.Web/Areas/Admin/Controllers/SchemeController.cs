namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Base;
    using Core.Scheme;
    using Infrastructure;
    using ViewModels;
    using Weee.Requests.Scheme;

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
            return View(new ManageSchemesViewModel { Schemes = await GetSchemes() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageSchemes(ManageSchemesViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(new ManageSchemesViewModel { Schemes = await GetSchemes() });
            }

            return RedirectToAction("ManageScheme", new { schemeId = viewModel.Selected.Value });
        }

        private async Task<List<SchemeData>> GetSchemes()
        {
            using (var client = apiClient())
            {
                return await client.SendAsync(User.GetAccessToken(), new GetSchemes());
            }
        }

        [HttpGet]
        public ViewResult ManageScheme(Guid schemeId)
        {
            // verify here that the user is allowed to look at the supplied scheme

            throw new NotImplementedException();
        }
    }
}