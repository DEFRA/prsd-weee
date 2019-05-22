namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.AatfReturn.Internal;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Admin.Requests;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using EA.Weee.Web.Areas.Admin.ViewModels.Home;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;

    public class AatfController : AdminController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IMapper mapper;
        private readonly IEditAatfDetailsRequestCreator detailsRequestCreator;
        private readonly IEditAatfContactRequestCreator contactRequestCreator;

        public AatfController(Func<IWeeeClient> apiClient, BreadcrumbService breadcrumb, IMapper mapper, IEditAatfDetailsRequestCreator detailsRequestCreator, IEditAatfContactRequestCreator contactRequestCreator)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.mapper = mapper;
            this.detailsRequestCreator = detailsRequestCreator;
            this.contactRequestCreator = contactRequestCreator;
        }

        [HttpGet]
        public async Task<ActionResult> Details(Guid id)
        {
            SetBreadcrumb();

            using (var client = apiClient())
            {
                AatfData aatf = await client.SendAsync(User.GetAccessToken(), new GetAatfById(id));
                AatfContactData contactData = await client.SendAsync(User.GetAccessToken(), new GetAatfContact(id));

                AatfDetailsViewModel viewModel = mapper.Map<AatfDetailsViewModel>(new AatfDataToAatfDetailsViewModelMapTransfer(aatf, contactData));

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
        public async Task<ActionResult> ManageAatfDetails(Guid id)
        {
            using (var client = apiClient())
            {
                var aatf = await client.SendAsync(User.GetAccessToken(), new GetAatfById(id));

                if (!aatf.CanEdit)
                {
                    return new HttpForbiddenResult();
                }

                var viewModel = mapper.Map<AatfEditDetailsViewModel>(aatf);
                var accessToken = User.GetAccessToken();
                viewModel.CompetentAuthoritiesList = await client.SendAsync(accessToken, new GetUKCompetentAuthorities());
                viewModel.SiteAddress.Countries = await client.SendAsync(accessToken, new GetCountries(false));

                SetBreadcrumb();
                return View(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageAatfDetails(AatfEditDetailsViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var client = apiClient())
                {
                    viewModel.CompetentAuthoritiesList = await client.SendAsync(User.GetAccessToken(), new GetUKCompetentAuthorities());
                    var request = detailsRequestCreator.ViewModelToRequest(viewModel);
                    await client.SendAsync(User.GetAccessToken(), request);
                }

                return Redirect(Url.Action("Details", new { area = "Admin", viewModel.Id }));
            }

            using (var client = apiClient())
            {
                var accessToken = User.GetAccessToken();
                viewModel.AatfStatusList = Enumeration.GetAll<AatfStatus>();
                viewModel.SizeList = Enumeration.GetAll<AatfSize>();
                viewModel.CompetentAuthoritiesList = await client.SendAsync(accessToken, new GetUKCompetentAuthorities());
                viewModel.SiteAddress.Countries = await client.SendAsync(accessToken, new GetCountries(false));
            }

            SetBreadcrumb();
            return View(viewModel);
        }

        [HttpGet]
        public async Task<ActionResult> ManageContactDetails(Guid id)
        {
            using (var client = apiClient())
            {
                var contact = await client.SendAsync(User.GetAccessToken(), new GetAatfContact(id));

                if (!contact.CanEditContactDetails)
                {
                    return new HttpForbiddenResult();
                }

                var viewModel = new AatfEditContactAddressViewModel()
                {
                    AatfId = id,
                    ContactData = contact
                };

                viewModel.ContactData.AddressData.Countries = await client.SendAsync(User.GetAccessToken(), new GetCountries(false));
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
                    var request = contactRequestCreator.ViewModelToRequest(viewModel);

                    await client.SendAsync(User.GetAccessToken(), request);

                    return Redirect(Url.Action("Details", new { area = "Admin", Id = viewModel.AatfId }) + "#contactDetails");
                }
            }

            using (var client = apiClient())
            {
                viewModel.ContactData.AddressData.Countries = await client.SendAsync(User.GetAccessToken(), new GetCountries(false));
            }

            SetBreadcrumb();

            return View(viewModel);
        }

        private async Task<List<AatfDataList>> GetAatfs()
        {
            using (var client = apiClient())
            {
                return await client.SendAsync(User.GetAccessToken(), new GetAatfs(FacilityType.Aatf));
            }
        }

        private void SetBreadcrumb()
        {
            breadcrumb.InternalActivity = InternalUserActivity.ManageAatfs;
        }
    }
}