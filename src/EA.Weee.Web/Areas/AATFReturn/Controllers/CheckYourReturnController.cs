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

    public class CheckYourReturnController : Controller
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IWeeeCache cache;
        private readonly BreadcrumbService breadcrumb;
        public int TonnageTotal;
        public int TonnageDcfTotal;

        public CheckYourReturnController(Func<IWeeeClient> apiClient,
            IWeeeCache cache,
            BreadcrumbService breadcrumb)
        {
            this.apiClient = apiClient;
            this.cache = cache;
            this.breadcrumb = breadcrumb;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index()
        {
            List<int> tonnageList;
            List<int> tonnageDcfList;
            Guid test = Guid.Parse("1952037B-BE7F-4515-BD01-A9E600FEBA78");

            using (var client = apiClient())
            {
                tonnageList = await client.SendAsync(User.GetAccessToken(), new FetchNonObligatedWeeeForReturnRequest(test, false));
                //tonnageDcfList = await client.SendAsync(User.GetAccessToken(), new FetchNonObligatedWeeeForReturnRequest(returnid, true));
            }

            CalculateListTotal(tonnageList, false);
            //CalculateListTotal(tonnageDcfList, true);

            var viewModel = new CheckYourReturnViewModel(TonnageTotal, TonnageDcfTotal);
            return View(viewModel);
        }

        [HttpPost]
        public virtual async Task<ActionResult> Index(CheckYourReturnViewModel viewModel)
        {
            return View(viewModel);
        }

        private void CalculateListTotal(List<int> list, bool dcf)
        {
            if (dcf)
            {
                foreach (var number in list)
                {
                    TonnageDcfTotal += number;
                }
            }
            else
            {
                foreach (var number in list)
                {
                    TonnageTotal += number;
                }
            }
        }
    }
}
