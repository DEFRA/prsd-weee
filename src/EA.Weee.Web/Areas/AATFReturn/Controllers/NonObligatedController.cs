namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Constant;
    using Core.AatfReturn;
    using FluentValidation;
    using Infrastructure;
    using Requests;
    using Services;
    using Services.Caching;
    using ViewModels;
    using ViewModels.Validation;
    using Web.Controllers.Base;

    public class NonObligatedController : ExternalSiteController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly INonObligatedWeeRequestCreator requestCreator;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly INonObligatedValuesViewModelValidatorWrapper validator;

        public NonObligatedController(IWeeeCache cache, BreadcrumbService breadcrumb, Func<IWeeeClient> apiClient, INonObligatedWeeRequestCreator requestCreator, INonObligatedValuesViewModelValidatorWrapper validator)
        {
            this.apiClient = apiClient;
            this.requestCreator = requestCreator;
            this.validator = validator;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid organisationId, Guid returnId, bool dcf)
        {
            var viewModel = new NonObligatedValuesViewModel(new NonObligatedCategoryValues()) { OrganisationId = organisationId, ReturnId = returnId, Dcf = dcf };

            await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfReturn);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(NonObligatedValuesViewModel viewModel)
        {
            var result = validator.Validate(viewModel, User.GetAccessToken(), apiClient);

            // CREATE FUNCTION TO DO THIS
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
            }

            if (ModelState.IsValid)
            {
                using (var client = apiClient())
                {
                    var request = requestCreator.ViewModelToRequest(viewModel);

                    await client.SendAsync(User.GetAccessToken(), request);

                    return RedirectToAction("Index", "AatfTaskList", new { area = "AatfReturn", organisationId = viewModel.OrganisationId, returnId = viewModel.ReturnId });
                }
            }
            await SetBreadcrumb(viewModel.OrganisationId, BreadCrumbConstant.AatfReturn);
            return View(viewModel);
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.SchemeInfo = await cache.FetchSchemePublicInfo(organisationId);
        }
    }
}