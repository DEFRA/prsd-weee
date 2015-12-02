namespace EA.Weee.Web.Areas.Scheme.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Core.DataReturns;
    using Core.Scheme;
    using Core.Shared;
    using Infrastructure;
    using Prsd.Core.Mapper;
    using Services;
    using Services.Caching;
    using ViewModels;
    using ViewModels.DataReturns;
    using Web.Controllers.Base;
    using Weee.Requests.DataReturns;
    using Weee.Requests.Organisations;
    using Weee.Requests.Scheme;

    public class DataReturnsController : ExternalSiteController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IWeeeCache cache;
        private readonly BreadcrumbService breadcrumb;
        private readonly CsvWriterFactory csvWriterFactory;
        private readonly IMapper mapper;

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

                await SetBreadcrumb(pcsId);

                return View(new AuthorizationRequiredViewModel
                {
                    Status = status,
                    ShowLinkToSelectOrganisation = showLinkToSelectOrganisation
                });
            }
        }

        [HttpGet]
        public async Task<ViewResult> Upload(Guid pcsId)
        {
            using (var client = apiClient())
            {
                var orgExists = await client.SendAsync(User.GetAccessToken(), new VerifyOrganisationExists(pcsId));
                if (orgExists)
                {
                    await SetBreadcrumb(pcsId);
                    return View();
                }
            }

            throw new InvalidOperationException(string.Format("'{0}' is not a valid organisation Id", pcsId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Upload(Guid pcsId, PCSFileUploadViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await SetBreadcrumb(pcsId);
                return View(model);
            }

            Guid dataReturnId;
            using (var client = apiClient())
            {
                model.PcsId = pcsId;
                var request = mapper.Map<PCSFileUploadViewModel, ProcessDataReturnsXMLFile>(model);
                dataReturnId = await client.SendAsync(User.GetAccessToken(), request);
            }

            DataReturnForSubmission dataReturn = await FetchDataReturn(pcsId, dataReturnId);
            if (dataReturn.Errors.Count == 0)
            {
                return RedirectToAction("Submit", new { pcsId = pcsId, dataReturnId = dataReturnId });
            }
            else
            {
                return RedirectToAction("Review", new { pcsId = pcsId, dataReturnId = dataReturnId });
            }
        }

        [HttpGet]
        public async Task<ActionResult> Review(Guid pcsId, Guid dataReturnId)
        {
            DataReturnForSubmission dataReturn = await FetchDataReturn(pcsId, dataReturnId);

            if (dataReturn.Errors.Count == 0)
            {
                return RedirectToAction("Submit", new { pcsId, dataReturnId });
            }

            SubmitViewModel viewModel = new SubmitViewModel()
            {
                DataReturn = dataReturn
            };

            await SetBreadcrumb(pcsId);

            return View("Submit", viewModel);
        }

        [HttpGet]
        public async Task<ActionResult> Submit(Guid pcsId, Guid dataReturnId)
        {
            DataReturnForSubmission dataReturn = await FetchDataReturn(pcsId, dataReturnId);

            if (dataReturn.Errors.Count != 0)
            {
                return RedirectToAction("Review", new { pcsId,  dataReturnId });
            }

            SubmitViewModel viewModel = new SubmitViewModel()
            {
                DataReturn = dataReturn
            };

            await SetBreadcrumb(pcsId);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Submit(Guid pcsId, Guid dataReturnId, SubmitViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                DataReturnForSubmission dataReturn = await FetchDataReturn(pcsId, dataReturnId);

                viewModel.DataReturn = dataReturn;

                return View(viewModel);
            }

            // TODO: Submit the data return.

            return RedirectToAction("SuccessfulSubmission", new { pcsId });
        }

        [HttpGet]
        public async Task<ActionResult> SuccessfulSubmission(Guid pcsId)
        {
            await SetBreadcrumb(pcsId);
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> DownloadErrorsAndWarnings(Guid pcsId, Guid dataReturnId)
        {
            SchemePublicInfo scheme = await cache.FetchSchemePublicInfo(pcsId);

            DataReturnForSubmission dataReturn = await FetchDataReturn(pcsId, dataReturnId);

            CsvWriter<IErrorOrWarning> csvWriter = csvWriterFactory.Create<IErrorOrWarning>();
            csvWriter.DefineColumn("Type", e => e.TypeName);
            csvWriter.DefineColumn("Description", e => e.Description);

            List<IErrorOrWarning> errorsAndWarnings = new List<IErrorOrWarning>();
            errorsAndWarnings.AddRange(dataReturn.Errors);
            errorsAndWarnings.AddRange(dataReturn.Warnings);

            string csv = csvWriter.Write(errorsAndWarnings);

            string filename = string.Format(
                "{0}_{1}{2}_data_return_errors_and_warnings_{3}.csv",
                scheme.ApprovalNo,
                dataReturn.Quarter.Year,
                dataReturn.Quarter.Q,
                DateTime.Now.ToString("ddMMyyyy_HHmm"));

            byte[] fileContent = Encoding.UTF8.GetBytes(csv);

            return File(fileContent, "text/csv", CsvFilenameFormat.FormatFileName(filename));
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

        private async Task SetBreadcrumb(Guid organisationId)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = "Submit a data return";
            breadcrumb.SchemeInfo = await cache.FetchSchemePublicInfo(organisationId);
        }

        private async Task<DataReturnForSubmission> FetchDataReturn(Guid pcsId, Guid dataReturnId)
        {
            DataReturnForSubmission dataReturn;

            using (var client = apiClient())
            {
                FetchDataReturnForSubmission request = new FetchDataReturnForSubmission(dataReturnId);
                dataReturn = await client.SendAsync(User.GetAccessToken(), request);
            }

            if (dataReturn.OrganisationId != pcsId)
            {
                string errorMessage = "The specified data return was not uploaded for the current organisation.";
                throw new InvalidOperationException(errorMessage);
            }

            return dataReturn;
        }
    }
}