namespace EA.Weee.Web.Areas.Test.Controllers
{
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.PCS.MemberUploadTesting;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Requests.PCS.MemberUploadTesting;
    using EA.Weee.Web.Areas.Test.ViewModels;
    using EA.Weee.Web.Areas.Test.ViewModels.GeneratePcsXml;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.ViewModels.Shared;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;

    [Authorize]
    public class GeneratePcsXmlController : Controller
    {
        private const int pageSize = 10;
        private readonly Func<IWeeeClient> apiClient;

        public GeneratePcsXmlController(Func<IWeeeClient> apiClient)
        {
            this.apiClient = apiClient;
        }

        [HttpGet]
        public async Task<ActionResult> SelectOrganisation(string companyName, int page = 1)
        {
            if (page < 1)
            {
                page = 1;
            }

            SelectOrganisationViewModel viewModel;

            if (string.IsNullOrEmpty(companyName))
            {
                viewModel = new SelectOrganisationViewModel();
            }
            else
            {
                var results = await FetchOrganisations(companyName, page);

                PagingViewModel pager = PagingViewModel.FromValues(
                    results.TotalMatchingOrganisations,
                    pageSize,
                    page,
                    "SelectOrganisation",
                    "GeneratePcsXml",
                    new { companyName });
                
                viewModel = new SelectOrganisationViewModel()
                {
                    CompanyName = companyName,
                    MatchingOrganisations = results.Results,
                    PagingViewModel = pager,
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
                IncludeMalformedSchema = viewModel.IncludeMalformedSchema,
                IncludeUnexpectedFooElement = viewModel.IncludeUnexpectedFooElement,
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
                return await client.SendAsync(User.GetAccessToken(), new FindMatchingOrganisations(companyName, page, pageSize));
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