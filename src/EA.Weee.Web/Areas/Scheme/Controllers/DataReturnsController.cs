namespace EA.Weee.Web.Areas.Scheme.Controllers
{
    using Api.Client;
    using Core.Shared;
    using Infrastructure;
    using Prsd.Core.Mapper;
    using Services;
    using Services.Caching;
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using ViewModels;
    using Web.Controllers.Base;
    using Weee.Requests.Organisations;
    using Weee.Requests.Scheme;

    public class DataReturnsController : ExternalSiteController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IWeeeCache cache;
        private readonly BreadcrumbService breadcrumb;
        private readonly CsvWriterFactory csvWriterFactory;
        private readonly IMapper mapper;
        private const string SubmitDataReturnsActivity = "Submit data returns";

        public DataReturnsController(
            Func<IWeeeClient> apiClient,
            IWeeeCache cache,
            BreadcrumbService breadcrumb,
            CsvWriterFactory csvWriterFactory,
            IMapper mapper)
        {
            this.apiClient = apiClient;
            this.cache = cache;
            this.breadcrumb = breadcrumb;
            this.csvWriterFactory = csvWriterFactory;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> AuthorisationRequired(Guid pcsId)
        {
            using (var client = apiClient())
            {
                var status = await client.SendAsync(User.GetAccessToken(), new GetSchemeStatus(pcsId));

                if (status == SchemeStatus.Approved)
                {
                    return RedirectToAction("SubmitDataReturns", "DataReturns");
                }

                var userIdString = User.GetUserId();
                var showLinkToSelectOrganisation = false;

                if (userIdString != null)
                {
                    var userId = new Guid(userIdString);

                    var task = Task.Run(async () =>
                    {
                        return await cache.FetchUserActiveCompleteOrganisationCount(userId);
                    });
                    task.Wait();

                    showLinkToSelectOrganisation = (task.Result > 1);
                }

                await SetBreadcrumb(pcsId, SubmitDataReturnsActivity);
                return View(new AuthorizationRequiredViewModel
                {
                    Status = status,
                    ShowLinkToSelectOrganisation = showLinkToSelectOrganisation
                });
            }
        }

        [HttpGet]
        public async Task<ViewResult> SubmitDataReturns(Guid pcsId)
        {
            using (var client = apiClient())
            {
                var orgExists = await client.SendAsync(User.GetAccessToken(), new VerifyOrganisationExists(pcsId));
                if (orgExists)
                {
                    await SetBreadcrumb(pcsId, SubmitDataReturnsActivity);
                    return View();
                }
            }

            throw new InvalidOperationException(string.Format("'{0}' is not a valid organisation Id", pcsId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SubmitDataReturns(Guid pcsId, PCSFileUploadViewModel model)
        {
            await SetBreadcrumb(pcsId, SubmitDataReturnsActivity);
            return View(model);
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.ActionDescriptor.ActionName == "AuthorisationRequired")
            {
                base.OnActionExecuting(filterContext);
            }
            else
            {
                object pcsIdObject;
                if (filterContext.ActionParameters.TryGetValue("pcsId", out pcsIdObject))
                {
                    SchemeStatus status;
                    Guid pcsId = (Guid)pcsIdObject;

                    using (var client = apiClient())
                    {
                        var schemeStatusTask = Task<SchemeStatus>.Run(() => client.SendAsync(User.GetAccessToken(), new GetSchemeStatus(pcsId)));
                        schemeStatusTask.Wait();

                        status = schemeStatusTask.Result;
                    }

                    if (status != SchemeStatus.Approved)
                    {
                        filterContext.Result = RedirectToAction("AuthorisationRequired", new { pcsID = pcsId });
                    }
                    else
                    {
                        base.OnActionExecuting(filterContext);
                    }
                }
                else
                {
                    throw new InvalidOperationException("The PCS ID could not be retrieved.");
                }
            }
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.SchemeInfo = await cache.FetchSchemePublicInfo(organisationId);
        }
    }
}