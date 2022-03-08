namespace EA.Weee.Web.Controllers
{
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.ViewModels.Organisation;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class OrganisationsController : Controller
    {
        private readonly Func<IWeeeClient> apiClient;

        public OrganisationsController(Func<IWeeeClient> apiClient)
        {
            this.apiClient = apiClient;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            return await ShowAllOrganisations();
        }

        private async Task<ActionResult> ShowAllOrganisations()
        {
            List<OrganisationNameStatus> organisationNameStatus;

            using (var client = apiClient())
            {
                organisationNameStatus = await
                 client.SendAsync(
                     User.GetAccessToken(),
                     new GetAllOrganisations());
            }

            var viewModel = new OrganisationNameStatusViewModel
            {
                 OrganisationsList = organisationNameStatus
            };

            return View("Index", viewModel);
        }
    }
}