﻿namespace EA.Weee.Web.Areas.AeReturn.Controllers
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Web.Areas.AatfReturn.Attributes;
    using EA.Weee.Web.Areas.AeReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.ViewModels.Returns;
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [ValidateOrganisationActionFilterAttribute(FacilityType = FacilityType.Ae)]
    public class ReturnsController : AeReturnBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IMapper mapper;

        public ReturnsController(Func<IWeeeClient> apiClient, BreadcrumbService breadcrumb, IWeeeCache cache, IMapper mapper)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.mapper = mapper;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid organisationId)
        {
            using (var client = apiClient())
            {
                var @returns = await client.SendAsync(User.GetAccessToken(), new GetReturns(organisationId, FacilityType.Ae));

                var viewModel = mapper.Map<ReturnsViewModel>(@returns);

                viewModel.OrganisationName = (await client.SendAsync(User.GetAccessToken(), new GetOrganisationInfo(organisationId))).OrganisationName;
                viewModel.OrganisationId = organisationId;

                await SetBreadcrumb(organisationId, BreadCrumbConstant.AeReturn);

                return View(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(ReturnsViewModel viewModel)
        {
            using (var client = apiClient())
            {
                AddReturn request = new AddReturn()
                {
                    OrganisationId = viewModel.OrganisationId,
                    FacilityType = FacilityType.Ae,
                    Quarter = viewModel.Quarter,
                    Year = viewModel.ComplianceYear
                };

                Guid returnId = await client.SendAsync(User.GetAccessToken(), request);

                return AeRedirect.ExportedWholeWeee(viewModel.OrganisationId, returnId);
            }
        }

        [HttpGet]
        public async Task<ActionResult> ExportedWholeWeee(Guid organisationId, Guid? returnId = null)
        {
            await SetBreadcrumb(organisationId, BreadCrumbConstant.AeReturn);

            ExportedWholeWeeeViewModel model = new ExportedWholeWeeeViewModel()
            {
                ReturnId = returnId.GetValueOrDefault(),
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExportedWholeWeee(Guid organisationId, ExportedWholeWeeeViewModel viewModel)
        {
            await SetBreadcrumb(organisationId, BreadCrumbConstant.AeReturn);

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            if (viewModel.WeeeSelectedValue == YesNoEnum.No)
            {
                return AeRedirect.ReturnsList(organisationId);
            }

            return AeRedirect.NilReturn(viewModel.ReturnId);
        }

        [HttpGet]
        public async Task<ActionResult> NilReturn(Guid returnId)
        {
            using (var client = apiClient())
            {
                var @return = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId, false));

                await SetBreadcrumb(@return.OrganisationData.Id, BreadCrumbConstant.AeReturn);

                return View(mapper.Map<SubmittedReturnViewModel>(@return));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> NilReturnConfirm(SubmittedReturnViewModel viewModel)
        {
            using (var client = apiClient())
            {
                await client.SendAsync(User.GetAccessToken(), new SubmitReturn(viewModel.ReturnId, true));

                return AeRedirect.Confirmation(viewModel.ReturnId);
            }
        }

        [HttpGet]
        public async Task<ActionResult> Confirmation(Guid returnId)
        {
            using (var client = apiClient())
            {
                var @return = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId, false));

                await SetBreadcrumb(@return.OrganisationData.Id, BreadCrumbConstant.AeReturn);

                return View(mapper.Map<SubmittedReturnViewModel>(@return));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Confirmation(SubmittedReturnViewModel model)
        {
            return await Task.Run<ActionResult>(() =>
                RedirectToAction("ChooseActivity", "Home", new { area = "Scheme", pcsId = model.OrganisationId }));
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.OrganisationId = organisationId;
        }
    }
}