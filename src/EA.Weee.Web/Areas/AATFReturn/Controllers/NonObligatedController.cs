namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Core.DataReturns;
    using Core.Scheme;
    using Core.Shared;
    using Infrastructure;
    using Microsoft.Owin.Security;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using Prsd.Core.Web.ApiClient;
    using Prsd.Core.Web.Mvc.Extensions;
    using Prsd.Core.Web.OAuth;
    using Services;
    using Services.Caching;
    using ViewModels;
    using Web.Controllers.Base;
    using Web.ViewModels.Shared;
    using Weee.Requests.AatfReturn;
    using Weee.Requests.DataReturns;
    using Weee.Requests.Organisations;
    using Weee.Requests.Scheme;
    using Weee.Requests.Shared;

    public class NonObligatedController : Controller
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IAuthenticationManager authenticationManager;
        private readonly Func<IOAuthClient> oauthClient;
        private readonly IExternalRouteService externalRouteService;
        private readonly IAppConfiguration appConfig;
        public List<NonObligatedRequest> NonObligatedRequestList;
        private readonly IMapper mapper;

        public NonObligatedController(IMapper mapper, Func<IOAuthClient> oauthClient, Func<IWeeeClient> apiClient, IAuthenticationManager authenticationManager, IExternalRouteService externalRouteService, IAppConfiguration appConfig)
        {
            this.oauthClient = oauthClient;
            this.apiClient = apiClient;
            this.authenticationManager = authenticationManager;
            this.externalRouteService = externalRouteService;
            this.appConfig = appConfig;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> Index(Guid organisationId)
        {
            var viewModel = new WeeeCategoryValueViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(WeeeCategoryValueViewModel viewModel)
        {
            using (var client = apiClient())
            {
                var request = mapper.Map<WeeeCategoryValueViewModel, NonObligatedRequest>(viewModel);
                await client.SendAsync(request);
                return View(viewModel);
            }
        }
    }
}