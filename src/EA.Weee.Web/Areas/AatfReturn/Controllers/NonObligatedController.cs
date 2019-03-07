namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Constant;
    using Core.AatfReturn;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.AatfReturn.NonObligated;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using FluentValidation;
    using FluentValidation.Results;
    using Infrastructure;
    using Requests;
    using Services;
    using Services.Caching;
    using ViewModels;
    using ViewModels.Validation;
    using Web.Controllers.Base;

    public class NonObligatedController : AatfReturnBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly INonObligatedWeeRequestCreator requestCreator;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly INonObligatedValuesViewModelValidatorWrapper validator;
        private readonly IMap<ReturnToNonObligatedValuesViewModelMapTransfer, NonObligatedValuesViewModel> mapper;

        public NonObligatedController(IWeeeCache cache, BreadcrumbService breadcrumb, Func<IWeeeClient> apiClient, INonObligatedWeeRequestCreator requestCreator, INonObligatedValuesViewModelValidatorWrapper validator, IMap<ReturnToNonObligatedValuesViewModelMapTransfer, NonObligatedValuesViewModel> mapper)
        {
            this.apiClient = apiClient;
            this.requestCreator = requestCreator;
            this.validator = validator;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.mapper = mapper;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid organisationId, Guid returnId, bool dcf)
        {
            using (var client = apiClient())
            {
                var @return = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId));

                var model = mapper.Map(new ReturnToNonObligatedValuesViewModelMapTransfer() { OrganisationId = organisationId, ReturnId = returnId, Dcf = dcf, ReturnData = @return });

                //var viewModel = new NonObligatedValuesViewModel(new NonObligatedCategoryValues()) { OrganisationId = organisationId, ReturnId = returnId, Dcf = dcf };

                await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfReturn);

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(NonObligatedValuesViewModel viewModel)
        {
            using (var client = apiClient())
            {
                if (ModelState.IsValid)
                {
                    await ValidateResult(viewModel, client);
                }

                if (ModelState.IsValid)
                {
                    var request = requestCreator.ViewModelToRequest(viewModel);

                    await client.SendAsync(User.GetAccessToken(), request);

                    return RedirectToAction("Index", "AatfTaskList", new { area = "AatfReturn", organisationId = viewModel.OrganisationId, returnId = viewModel.ReturnId });
                }

                await SetBreadcrumb(viewModel.OrganisationId, BreadCrumbConstant.AatfReturn);

                return View(viewModel);
            }
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.SchemeInfo = await cache.FetchSchemePublicInfo(organisationId);
        }

        private async Task ValidateResult(NonObligatedValuesViewModel model, IWeeeClient client)
        {
            var @return = await client.SendAsync(User.GetAccessToken(), new GetReturn(model.ReturnId));
            var result = await validator.Validate(model, @return);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
            }
        }
    }
}