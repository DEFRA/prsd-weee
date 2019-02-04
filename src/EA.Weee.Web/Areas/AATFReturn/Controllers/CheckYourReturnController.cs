namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Core.AatfReturn;
    using EA.Weee.Requests.AatfReturn;
    using Infrastructure;
    using Microsoft.Owin.Security;
    using Prsd.Core.Mapper;
    using Prsd.Core.Web.OAuth;
    using Requests;
    using Services;
    using Services.Caching;
    using ViewModels;
    using Web.Controllers.Base;
    using Weee.Requests.AatfReturn.NonObligated;

    public class CheckYourReturnController : Controller
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IWeeeCache cache;
        private readonly BreadcrumbService breadcrumb;
        public decimal? TonnageTotal = 0.000m;
        public decimal? TonnageDcfTotal = 0.000m;

        public CheckYourReturnController(Func<IWeeeClient> apiClient,
            IWeeeCache cache,
            BreadcrumbService breadcrumb)
        {
            this.apiClient = apiClient;
            this.cache = cache;
            this.breadcrumb = breadcrumb;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid returnid, Guid organisationid)
        {
            List<decimal?> tonnageList;
            List<decimal?> tonnageDcfList;

            using (var client = apiClient())
            {
                tonnageList = await client.SendAsync(User.GetAccessToken(), new FetchNonObligatedWeeeForReturnRequest(returnid, organisationid, false));
                tonnageDcfList = await client.SendAsync(User.GetAccessToken(), new FetchNonObligatedWeeeForReturnRequest(returnid, organisationid, true));
            }

            CalculateListTotal(tonnageList, false);
            CalculateListTotal(tonnageDcfList, true);

            var viewModel = new CheckYourReturnViewModel(TonnageTotal, TonnageDcfTotal);
            return View(viewModel);
        }

        [HttpPost]
        public virtual async Task<ActionResult> Index(CheckYourReturnViewModel viewModel)
        {
            return View(viewModel);
        }

        private void CalculateListTotal(List<decimal?> list, bool dcf)
        {
            if (dcf)
            {
                foreach (var number in list)
                {
                    if (number != null)
                    {
                        TonnageDcfTotal += number;
                    }
                }
            }
            else
            {
                foreach (var number in list)
                {
                    if (number != null)
                    {
                        TonnageTotal += number;
                    }
                }
            }
        }
    }
}
