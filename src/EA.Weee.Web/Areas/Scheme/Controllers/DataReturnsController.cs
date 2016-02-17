namespace EA.Weee.Web.Areas.Scheme.Controllers
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
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using Services;
    using Services.Caching;
    using ViewModels;
    using ViewModels.DataReturns;
    using Web.Controllers.Base;
    using Weee.Requests.DataReturns;
    using Weee.Requests.Organisations;
    using Weee.Requests.Scheme;
    using Weee.Requests.Shared;

    public class DataReturnsController : ExternalSiteController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IWeeeCache cache;
        private readonly BreadcrumbService breadcrumb;
        private readonly CsvWriterFactory csvWriterFactory;
        private readonly IMapper mapper;
        private readonly ConfigurationService configService;

        public DataReturnsController(
            Func<IWeeeClient> apiClient,
            IWeeeCache cache,
            BreadcrumbService breadcrumb,
            CsvWriterFactory csvWriterFactory,
            IMapper mapper,
            ConfigurationService configService)
        {
            this.apiClient = apiClient;
            this.cache = cache;
            this.breadcrumb = breadcrumb;
            this.csvWriterFactory = csvWriterFactory;
            this.mapper = mapper;
            this.configService = configService;
        }

        [HttpGet]
        public async Task<ActionResult> AuthorisationRequired(Guid pcsId)
        {
            using (IWeeeClient client = apiClient())
            {
                SchemeStatus status = await client.SendAsync(User.GetAccessToken(), new GetSchemeStatus(pcsId));

                if (status == SchemeStatus.Approved)
                {
                    return RedirectToAction("Index", new { pcsId });
                }

                string userIdString = User.GetUserId();
                bool showLinkToSelectOrganisation = false;

                if (userIdString != null)
                {
                    Guid userId = new Guid(userIdString);

                    int activeUserCompleteOrganisationCount = await cache.FetchUserActiveCompleteOrganisationCount(userId);

                    showLinkToSelectOrganisation = (activeUserCompleteOrganisationCount > 1);
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
        public async Task<ActionResult> Index(Guid pcsId)
        {
            await SetBreadcrumb(pcsId);

            List<int> complianceYears;
            using (var client = apiClient())
            {
                complianceYears = await client.SendAsync(User.GetAccessToken(), new FetchDataReturnComplianceYearsForScheme(pcsId));
            }

            IndexViewModel viewModel = new IndexViewModel(pcsId, complianceYears);
            return View(viewModel);
        }

        [HttpGet]
        public async Task<ActionResult> Upload(Guid pcsId)
        {
            await SetBreadcrumb(pcsId);
            using (var client = apiClient())
            {
                var isSubmissionWindowOpen = await client.SendAsync(User.GetAccessToken(), new IsSubmissionWindowOpen());

                if (isSubmissionWindowOpen)
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("CannotSubmitDataReturn", new { pcsId });
                }
            }
        }

        [HttpGet]
        public async Task<ViewResult> CannotSubmitDataReturn(Guid pcsId)
        {
            using (var client = apiClient())
            {
                var currentDate = await client.SendAsync(User.GetAccessToken(), new GetApiDate());

                await SetBreadcrumb(pcsId);
                return View(new CannotSubmitDataReturnViewModel
                {
                    OrganisationId = pcsId,
                    CurrentYear = currentDate.Year
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Upload(Guid pcsId, PCSFileUploadViewModel model)
        {
            if (!ModelState.IsValid)
            {
                if (Request.IsAjaxRequest())
                {
                    return new HttpStatusCodeResult(500);
                }
                else
                {
                    await SetBreadcrumb(pcsId);
                    return View(model);
                }
            }

            Guid dataReturnUploadId;
            using (var client = apiClient())
            {
                model.PcsId = pcsId;
                var request = mapper.Map<PCSFileUploadViewModel, ProcessDataReturnXmlFile>(model);
                dataReturnUploadId = await client.SendAsync(User.GetAccessToken(), request);
            }

            if (Request.IsAjaxRequest())
            {
                return Json(dataReturnUploadId);
            }
            else
            {
                DataReturnForSubmission dataReturn = await FetchDataReturnUpload(pcsId, dataReturnUploadId);
                if (dataReturn.Errors.Count == 0)
                {
                    return RedirectToAction("Submit", new { pcsId = pcsId, dataReturnUploadId = dataReturnUploadId });
                }
                else
                {
                    return RedirectToAction("Review", new { pcsId = pcsId, dataReturnUploadId = dataReturnUploadId });
                }
            }
        }

        [HttpGet]
        public async Task<ActionResult> Review(Guid pcsId, Guid dataReturnUploadId)
        {
            await SetBreadcrumb(pcsId);
            DataReturnForSubmission dataReturn = await FetchDataReturnUpload(pcsId, dataReturnUploadId);

            if (dataReturn.Errors.Count == 0)
            {
                return RedirectToAction("Submit", new { pcsId, dataReturnUploadId });
            }

            SubmitViewModel viewModel = new SubmitViewModel()
            {
                DataReturn = dataReturn,
                PcsId = pcsId
            };

            return View("Submit", viewModel);
        }

        [HttpGet]
        public async Task<ActionResult> Submit(Guid pcsId, Guid dataReturnUploadId)
        {
            await SetBreadcrumb(pcsId);
            DataReturnForSubmission dataReturn = await FetchDataReturnUpload(pcsId, dataReturnUploadId);

            if (dataReturn.Errors.Count != 0)
            {
                return RedirectToAction("Review", new { pcsId, dataReturnUploadId });
            }

            SubmitViewModel viewModel = new SubmitViewModel()
            {
                DataReturn = dataReturn,
                PcsId = pcsId
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Submit(Guid pcsId, Guid dataReturnUploadId, SubmitViewModel viewModel)
        {
            await SetBreadcrumb(pcsId);

            if (!ModelState.IsValid)
            {
                DataReturnForSubmission dataReturn = await FetchDataReturnUpload(pcsId, dataReturnUploadId);
                viewModel.DataReturn = dataReturn;
                viewModel.PcsId = pcsId;

                return View(viewModel);
            }

            using (var client = apiClient())
            {
                await client.SendAsync(User.GetAccessToken(), new SubmitDataReturnUpload(dataReturnUploadId));
            }

            return RedirectToAction("SuccessfulSubmission", new { pcsId, dataReturnUploadId });
        }

        [HttpGet]
        public async Task<ActionResult> SuccessfulSubmission(Guid pcsId, Guid dataReturnUploadId)
        {
            await SetBreadcrumb(pcsId);
            using (var client = apiClient())
            {
                var quarterInfo =
                    await
                        client.SendAsync(User.GetAccessToken(),
                            new GetQuarterInfoByDataReturnUploadId(dataReturnUploadId));

                var quarterText = string.Empty;

                if (quarterInfo.Quarter.Value == QuarterType.Q1)
                {
                    quarterText = "Quarter 1";
                }
                else if (quarterInfo.Quarter.Value == QuarterType.Q2)
                {
                    quarterText = "Quarter 2";
                }
                else if (quarterInfo.Quarter.Value == QuarterType.Q3)
                {
                    quarterText = "Quarter 3";
                }
                else if (quarterInfo.Quarter.Value == QuarterType.Q4)
                {
                    quarterText = "Quarter 4";
                }

                return View(new ViewModels.DataReturns.SuccessfulSubmissionViewModel
                {
                    PcsId = pcsId,
                    ComplianceYear = quarterInfo.Year.Value,
                    QuarterText = quarterText
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult> DownloadErrorsAndWarnings(Guid pcsId, Guid dataReturnUploadId)
        {
            SchemePublicInfo scheme = await cache.FetchSchemePublicInfo(pcsId);

            DataReturnForSubmission dataReturn = await FetchDataReturnUpload(pcsId, dataReturnUploadId);

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
                dataReturn.Year,
                dataReturn.Quarter,
                DateTime.Now.ToString("ddMMyyyy_HHmm"));

            byte[] fileContent = Encoding.UTF8.GetBytes(csv);

            return File(fileContent, "text/csv", CsvFilenameFormat.FormatFileName(filename));
        }

        [HttpGet]
        public async Task<ActionResult> DownloadEeeWeeeData(Guid pcsId, int complianceYear)
        {
            FileInfo file;
            using (IWeeeClient client = apiClient())
            {
                FetchSummaryCsv fetchSummaryCsv = new FetchSummaryCsv(pcsId, complianceYear);
                file = await client.SendAsync(User.GetAccessToken(), fetchSummaryCsv);
            }

            return File(file.Data, "text/csv", CsvFilenameFormat.FormatFileName(file.FileName));
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Ensure that Data Returns are enabled.
            if (!configService.CurrentConfiguration.EnableDataReturns)
            {
                throw new InvalidOperationException("Data returns are not enabled.");
            }

            // Ensure a organisation ID has been provided and that the organisation exists.
            object organisationIdActionParameter;
            if (!filterContext.ActionParameters.TryGetValue("pcsId", out organisationIdActionParameter))
            {
                throw new ArgumentException("No organisation ID was specified.");
            }

            if (!(organisationIdActionParameter is Guid))
            {
                throw new ArgumentException("The specified organisation ID is not valid.");
            }

            Guid organisationId = (Guid)organisationIdActionParameter;

            bool organisationExists;
            using (IWeeeClient client = apiClient())
            {
                Task<bool> task = Task.Run(() => client.SendAsync(User.GetAccessToken(), new VerifyOrganisationExists(organisationId)));
                task.Wait();
                organisationExists = task.Result;
            }

            if (!organisationExists)
            {
                throw new ArgumentException(string.Format("'{0}' is not a valid organisation Id.", organisationId));
            }

            /* Check whether the scheme representing the organisation has a status of "Approved".
             * If not, redirect the user to the "AuthorisationRequired" action (unless they are
             * already executing that action).
             */
            if (filterContext.ActionDescriptor.ActionName != "AuthorisationRequired")
            {
                SchemeStatus status;
                using (IWeeeClient client = apiClient())
                {
                    Task<SchemeStatus> schemeStatusTask = Task.Run(() => client.SendAsync(User.GetAccessToken(), new GetSchemeStatus(organisationId)));
                    schemeStatusTask.Wait();
                    status = schemeStatusTask.Result;
                }

                if (status != SchemeStatus.Approved)
                {
                    filterContext.Result = RedirectToAction("AuthorisationRequired", new { organisationId });
                    return;
                }
            }

            base.OnActionExecuting(filterContext);
        }

        private async Task SetBreadcrumb(Guid organisationId)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = "Manage EEE/WEEE data";
            breadcrumb.SchemeInfo = await cache.FetchSchemePublicInfo(organisationId);
        }

        private async Task<DataReturnForSubmission> FetchDataReturnUpload(Guid pcsId, Guid dataReturnUploadId)
        {
            DataReturnForSubmission dataReturn;

            using (var client = apiClient())
            {
                FetchDataReturnForSubmission request = new FetchDataReturnForSubmission(dataReturnUploadId);
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