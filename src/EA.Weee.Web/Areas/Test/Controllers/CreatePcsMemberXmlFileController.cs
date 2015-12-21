namespace EA.Weee.Web.Areas.Test.Controllers
{
    using System;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Core.Organisations;
    using Core.Scheme.MemberUploadTesting;
    using Core.Shared.Paging;
    using Infrastructure;
    using Services;
    using ViewModels.CreatePcsMemberXmlFile;
    using Weee.Requests.Organisations;
    using Weee.Requests.Scheme.MemberUploadTesting;

    [Authorize]
    public class CreatePcsMemberXmlFileController : Controller
    {
        private const int pageSize = 10;
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;

        public CreatePcsMemberXmlFileController(
            Func<IWeeeClient> apiClient,
            BreadcrumbService breadcrumb)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            breadcrumb.TestAreaActivity = "Create PCS member XML file";
        }

        [HttpGet]
        public async Task<ActionResult> SelectOrganisation(string organisationName, int page = 1)
        {
            if (page < 1)
            {
                page = 1;
            }

            SelectOrganisationViewModel viewModel;

            if (string.IsNullOrEmpty(organisationName))
            {
                viewModel = new SelectOrganisationViewModel();
            }
            else
            {
                var results = await FetchOrganisations(organisationName, page);
                var matchingOrganistionList = results.Results.ToPagedList(page - 1, pageSize,
                    results.TotalMatchingOrganisations);

                viewModel = new SelectOrganisationViewModel()
                {
                    OrganisationName = organisationName,
                    MatchingOrganisations = matchingOrganistionList
                };
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<ActionResult> SpecifyOptions(Guid organisationID)
        {
            if (!await CheckOrganisationExists(organisationID))
            {
                return RedirectToAction("SelectOrganisation");
            }

            SpecifyOptionsViewModel viewModel = new SpecifyOptionsViewModel()
            {
                OrganisationID = organisationID
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SpecifyOptions(SpecifyOptionsViewModel viewModel)
        {
            if (!await CheckOrganisationExists(viewModel.OrganisationID))
            {
                return RedirectToAction("SelectOrganisation");
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            return View("DownloadFile", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DownloadFile(SpecifyOptionsViewModel viewModel)
        {
            if (!await CheckOrganisationExists(viewModel.OrganisationID))
            {
                return RedirectToAction("SelectOrganisation");
            }

            if (!ModelState.IsValid)
            {
                return RedirectToAction("SpecifyOptions", new { viewModel.OrganisationID });
            }

            ProducerListSettings settings = new ProducerListSettings()
            {
                OrganisationID = viewModel.OrganisationID,
                SchemaVersion = viewModel.SchemaVersion,
                ComplianceYear = viewModel.ComplianceYear,
                NumberOfNewProducers = viewModel.NumberOfNewProducers,
                NumberOfExistingProducers = viewModel.NumberOfExistingProducers,
                NoCompaniesForNewProducers = viewModel.NoCompaniesForNewProducers,
                IncludeMalformedSchema = viewModel.IncludeMalformedSchema,
                IncludeUnexpectedFooElement = viewModel.IncludeUnexpectedFooElement,
                IgnoreStringLengthConditions = viewModel.IgnoreStringLengthConditions,
            };

            PcsXmlFile xmlFile = await GenerateXml(settings);

            ContentDisposition cd = new ContentDisposition
            {
                FileName = xmlFile.FileName,
                Inline = false,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(xmlFile.Data, "application/xml");
        }

        private async Task<bool> CheckOrganisationExists(Guid organisationID)
        {
            using (IWeeeClient client = apiClient())
            {
                return await client.SendAsync(User.GetAccessToken(), new VerifyOrganisationExists(organisationID));
            }
        }

        private async Task<OrganisationSearchDataResult> FetchOrganisations(string companyName, int? page)
        {
            using (IWeeeClient client = apiClient())
            {
                return await client.SendAsync(User.GetAccessToken(), new FindMatchingOrganisations(companyName));
            }
        }

        private async Task<PcsXmlFile> GenerateXml(ProducerListSettings settings)
        {
            using (IWeeeClient client = apiClient())
            {
                return await client.SendAsync(
                    User.GetAccessToken(),
                    new GeneratePcsXmlFile(settings));
            }
        }
    }
}