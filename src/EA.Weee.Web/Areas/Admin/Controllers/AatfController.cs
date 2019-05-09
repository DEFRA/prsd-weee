namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.AatfReturn.Internal;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Requests.Scheme;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.Requests;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using EA.Weee.Web.Areas.Admin.ViewModels.Home;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class AatfController : AdminController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IWeeeCache cache;
        private readonly BreadcrumbService breadcrumb;
        private readonly IMapper mapper;
        private readonly IEditAatfContactRequestCreator requestCreator;

        public AatfController(Func<IWeeeClient> apiClient, IWeeeCache cache, BreadcrumbService breadcrumb, IMapper mapper, IEditAatfContactRequestCreator requestCreator)
        {
            this.apiClient = apiClient;
            this.cache = cache;
            this.breadcrumb = breadcrumb;
            this.mapper = mapper;
            this.requestCreator = requestCreator;
        }

        [HttpGet]
        public async Task<ActionResult> Details(Guid id)
        {
            SetBreadcrumb();

            using (var client = apiClient())
            {
                AatfData aatf = await client.SendAsync(User.GetAccessToken(), new GetAatfById(id));

                AatfDetailsViewModel viewModel = mapper.Map<AatfDetailsViewModel>(aatf);

                return View(viewModel);
            }
        }
        
        public async Task<ActionResult> ManageAatfs()
        {
            SetBreadcrumb();
            return View(new ManageAatfsViewModel { AatfDataList = await GetAatfs() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageAatfs(ManageAatfsViewModel viewModel)
        {
            SetBreadcrumb();

            if (!ModelState.IsValid)
            {
                using (var client = apiClient())
                {
                    viewModel = new ManageAatfsViewModel
                    {
                        AatfDataList = await GetAatfs()
                    };
                    return View(viewModel);
                }    
            }
            else
            {
                return RedirectToAction("Details", new { Id = viewModel.Selected.Value });
            }           
        }

        [HttpGet]
        public async Task<ActionResult> ManageContactDetails(Guid aatfId)
        {
            using (var client = apiClient())
            {
                var contact = await client.SendAsync(User.GetAccessToken(), new GetAatfContact(aatfId));
                AatfEditContactAddressViewModel viewModel = new AatfEditContactAddressViewModel()
                {
                    AatfId = aatfId,
                    ContactData = contact
                };

                viewModel.ContactData.Countries = await client.SendAsync(User.GetAccessToken(), new GetCountries(false));
                SetBreadcrumb();
                return View(viewModel);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageContactDetails(AatfEditContactAddressViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var client = apiClient())
                {
                    var request = requestCreator.ViewModelToRequest(viewModel);

                    await client.SendAsync(User.GetAccessToken(), request);

                    viewModel.ContactData.Countries = await client.SendAsync(User.GetAccessToken(), new GetCountries(false));

                    return View(viewModel);
                }
            }

            SetBreadcrumb();
            using (var client = apiClient())
            {
                viewModel.ContactData.Countries = await client.SendAsync(User.GetAccessToken(), new GetCountries(false));
            }

            SetBreadcrumb();

            return View(viewModel);
        }

        private async Task<List<AatfDataList>> GetAatfs()
        {
            using (var client = apiClient())
            {
                return await client.SendAsync(User.GetAccessToken(), new GetAatfs());
            }
        }

        private void SetBreadcrumb()
        {
            breadcrumb.InternalActivity = InternalUserActivity.ManageAatfs;
        }
    }
}