namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Requests.Scheme;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
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

        public AatfController(Func<IWeeeClient> apiClient, IWeeeCache cache, BreadcrumbService breadcrumb, IMapper mapper)
        {
            this.apiClient = apiClient;
            this.cache = cache;
            this.breadcrumb = breadcrumb;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> Details(Guid id)
        {
            SetBreadcrumb();

            AatfData aatf;

            using (var client = apiClient())
            {
                aatf = await client.SendAsync(User.GetAccessToken(), new GetAatfById(id));
            }

            AatfDetailsViewModel viewModel = new AatfDetailsViewModel()
            {
                AatfId = id,
                AatfName = aatf.Name,
                ApprovalNumber = aatf.ApprovalNumber,
                CompetentAuthority = aatf.CompetentAuthority,
                AatfStatus = aatf.AatfStatus,
                SiteAddress = aatf.SiteAddress,
                Size = aatf.Size
            };

            if (aatf.ApprovalDate != default(DateTime))
            {
                viewModel.ApprovalDate = aatf.ApprovalDate.GetValueOrDefault();
            }
            
            return View(viewModel);
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