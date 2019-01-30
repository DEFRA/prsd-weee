namespace EA.Weee.Web.Areas.Scheme.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Core.Organisations;
    using Core.Shared.Paging;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.Scheme;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using Infrastructure;
    using Prsd.Core.Extensions;
    using ViewModels;
    using Web.Controllers.Base;
    using Web.ViewModels.Shared;
    using Web.ViewModels.Shared.Scheme;
    using Web.ViewModels.Shared.Submission;
    using Weee.Requests.Organisations;
    using Weee.Requests.Scheme.MemberRegistration;
    using Weee.Requests.Users;
    using Weee.Requests.Users.GetManageableOrganisationUsers;

    [Authorize]
    public class HomeController : ExternalSiteController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IWeeeCache cache;
        private readonly BreadcrumbService breadcrumb;
        private readonly CsvWriterFactory csvWriterFactory;
        private const string DoNotChange = "Do not change at this time";
        private readonly ConfigurationService configurationService;
        private const int DefaultPageSize = 15;

        public HomeController(Func<IWeeeClient> apiClient, IWeeeCache cache, BreadcrumbService breadcrumb, CsvWriterFactory csvWriterFactory, ConfigurationService configService)
        {
            this.apiClient = apiClient;
            this.cache = cache;
            this.breadcrumb = breadcrumb;
            this.csvWriterFactory = csvWriterFactory;
            this.configurationService = configService;
        }

        [HttpGet]
        public async Task<ActionResult> ChooseActivity(Guid pcsId)
        {
            using (var client = apiClient())
            {
                var organisationExists =
                    await client.SendAsync(User.GetAccessToken(), new VerifyOrganisationExists(pcsId));

                if (!organisationExists)
                {
                    throw new ArgumentException("No organisation found for supplied organisation Id", "organisationId");
                }

                List<string> activities = await GetActivities(pcsId);

                var model = new ChooseActivityViewModel(activities);
                model.OrganisationId = pcsId;
                await SetBreadcrumb(pcsId, null);

                await SetShowLinkToCreateOrJoinOrganisation(model);

                return View(model);
            }
        }

        internal async Task<List<string>> GetActivities(Guid pcsId)
        {
            string organisationDetailsActivityName;
            using (var client = apiClient())
            {
                var organisationDetails = await client.SendAsync(User.GetAccessToken(), new GetOrganisationInfo(pcsId));
                //Get the organisation type based on organisation id
                organisationDetailsActivityName = organisationDetails.OrganisationType == OrganisationType.RegisteredCompany ? PcsAction.ViewRegisteredOfficeDetails : PcsAction.ViewPrinciplePlaceOfBusinessDetails;
            }
            var organisationOverview = await GetOrganisationOverview(pcsId);

            List<string> activities = new List<string>();
            activities.Add(PcsAction.ManagePcsMembers);

            if (configurationService.CurrentConfiguration.EnableDataReturns)
            {
                activities.Add(PcsAction.ManageEeeWeeeData);
            }

            bool canDisplayDataReturnsHistory = organisationOverview.HasDataReturnSubmissions && configurationService.CurrentConfiguration.EnableDataReturns;
            if (organisationOverview.HasMemberSubmissions || canDisplayDataReturnsHistory)
            {
                activities.Add(PcsAction.ViewSubmissionHistory);
            }

            activities.Add(organisationDetailsActivityName);
            activities.Add(PcsAction.ManageContactDetails);

            if (organisationOverview.HasMultipleOrganisationUsers)
            {
                activities.Add(PcsAction.ManageOrganisationUsers);
            }

            if (configurationService.CurrentConfiguration.EnableAATFReturns)
            {
                activities.Add(PcsAction.MakeAATFReturn);
            }

            return activities;
        }

        private async Task<OrganisationOverview> GetOrganisationOverview(Guid organisationId)
        {
            using (var client = apiClient())
            {
                return await client.SendAsync(User.GetAccessToken(), new GetOrganisationOverview(organisationId));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChooseActivity(ChooseActivityViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.SelectedValue == PcsAction.ManagePcsMembers)
                {
                    using (var client = apiClient())
                    {
                        var status = await client.SendAsync(User.GetAccessToken(), new GetSchemeStatus(viewModel.OrganisationId));

                        if (status == SchemeStatus.Approved)
                        {
                            return RedirectToAction("Summary", "MemberRegistration", new { pcsId = viewModel.OrganisationId });
                        }
                        else
                        {
                            return RedirectToAction("AuthorisationRequired", "MemberRegistration", new { pcsId = viewModel.OrganisationId });
                        }
                    }
                }
                if (viewModel.SelectedValue == PcsAction.ManageOrganisationUsers)
                {
                    return RedirectToAction("ManageOrganisationUsers", new { pcsId = viewModel.OrganisationId });
                }
                if (viewModel.SelectedValue == PcsAction.ViewRegisteredOfficeDetails || viewModel.SelectedValue == PcsAction.ViewPrinciplePlaceOfBusinessDetails)
                {
                    return RedirectToAction("ViewOrganisationDetails", new { pcsId = viewModel.OrganisationId });
                }
                if (viewModel.SelectedValue == PcsAction.ManageContactDetails)
                {
                    return RedirectToAction("ManageContactDetails", new { pcsId = viewModel.OrganisationId });
                }
                if (viewModel.SelectedValue == PcsAction.ViewSubmissionHistory)
                {
                    var organisationOverview = await GetOrganisationOverview(viewModel.OrganisationId);

                    bool canViewDataReturnsSubmission = organisationOverview.HasDataReturnSubmissions && configurationService.CurrentConfiguration.EnableDataReturns;
                    if (organisationOverview.HasMemberSubmissions && canViewDataReturnsSubmission)
                    {
                        return RedirectToAction("ChooseSubmissionType", new { pcsId = viewModel.OrganisationId });
                    }
                    else if (organisationOverview.HasMemberSubmissions)
                    {
                        return RedirectToAction("ViewSubmissionHistory", new { pcsId = viewModel.OrganisationId });
                    }
                    else if (canViewDataReturnsSubmission)
                    {
                        return RedirectToAction("ViewDataReturnSubmissionHistory", new { pcsId = viewModel.OrganisationId });
                    }
                }
                if (viewModel.SelectedValue == PcsAction.ManageEeeWeeeData)
                {
                    return RedirectToAction("Index", "DataReturns", new { pcsId = viewModel.OrganisationId });
                }
                if (viewModel.SelectedValue == PcsAction.MakeAATFReturn)
                {
                    return RedirectToAction("Index", "NonObligated", new { area = "AatfReturn", organisationId = viewModel.OrganisationId, dcf = false });
                }
            }

            await SetBreadcrumb(viewModel.OrganisationId, null);
            viewModel.PossibleValues = await GetActivities(viewModel.OrganisationId);
            await SetShowLinkToCreateOrJoinOrganisation(viewModel);
            return View(viewModel);
        }

        [HttpGet]
        public async Task<ActionResult> ChooseSubmissionType(Guid pcsId)
        {
            using (var client = apiClient())
            {
                var organisationExists =
                    await client.SendAsync(User.GetAccessToken(), new VerifyOrganisationExists(pcsId));

                if (!organisationExists)
                {
                    throw new ArgumentException("No organisation found for supplied organisation Id", "organisationId");
                }

                var model = new ChooseSubmissionTypeViewModel
                {
                    PossibleValues = new List<string>
                    {
                        SubmissionType.MemberRegistrations,
                        SubmissionType.EeeOrWeeeDataReturns
                    },
                    OrganisationId = pcsId
                };

                await SetBreadcrumb(pcsId, "View submission history");

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChooseSubmissionType(ChooseSubmissionTypeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.SelectedValue == SubmissionType.EeeOrWeeeDataReturns)
                {
                    return RedirectToAction("ViewDataReturnSubmissionHistory", new { pcsId = viewModel.OrganisationId });
                }
                else if (viewModel.SelectedValue == SubmissionType.MemberRegistrations)
                {
                    return RedirectToAction("ViewSubmissionHistory", new { pcsId = viewModel.OrganisationId });
                }
            }

            await SetBreadcrumb(viewModel.OrganisationId, "View submission history");
            viewModel.PossibleValues = new List<string>
            {
                SubmissionType.MemberRegistrations,
                SubmissionType.EeeOrWeeeDataReturns
            };

            return View(viewModel);
        }

        private async Task SetShowLinkToCreateOrJoinOrganisation(ChooseActivityViewModel model)
        {
            IEnumerable<OrganisationUserData> organisations = await GetOrganisations();

            List<OrganisationUserData> accessibleOrganisations = organisations
                .Where(o => o.UserStatus == UserStatus.Active)
                .ToList();

            List<OrganisationUserData> inaccessibleOrganisations = organisations
                .Except(accessibleOrganisations)
                .ToList();

            bool showLink = (accessibleOrganisations.Count == 1 && inaccessibleOrganisations.Count == 0);

            model.ShowLinkToCreateOrJoinOrganisation = showLink;
        }

        /// <summary>
        /// Returns all complete organisations with which the user is associated.
        /// </summary>
        /// <returns></returns>
        private async Task<IEnumerable<OrganisationUserData>> GetOrganisations()
        {
            using (var client = apiClient())
            {
                return await
                 client.SendAsync(
                     User.GetAccessToken(),
                     new GetUserOrganisationsByStatus(new int[0], new int[1] { (int)OrganisationStatus.Complete }));
            }
        }

        [HttpGet]
        public async Task<ActionResult> ManageOrganisationUsers(Guid pcsId)
        {
            using (var client = apiClient())
            {
                var organisationExists =
                    await client.SendAsync(User.GetAccessToken(), new VerifyOrganisationExists(pcsId));

                if (!organisationExists)
                {
                    throw new ArgumentException("No organisation found for supplied organisation Id", "pcsId");
                }

                var orgUsers = await GetOrganisationUsers(pcsId);

                var orgUsersKeyValuePairs =
                    orgUsers.Select(
                        ou =>
                            new KeyValuePair<string, Guid>(
                                ou.User.FirstName + " " + ou.User.Surname + " (" +
                                ou.UserStatus.ToString() + ")", ou.Id));

                await SetBreadcrumb(pcsId, "Manage organisation users");

                var model = new OrganisationUsersViewModel
                {
                    OrganisationUsers = orgUsersKeyValuePairs.ToList()
                };

                return View("ManageOrganisationUsers", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageOrganisationUsers(Guid pcsId, OrganisationUsersViewModel model)
        {
            await SetBreadcrumb(pcsId, "Manage organisation users");

            if (!ModelState.IsValid)
            {
                var orgUsers = await GetOrganisationUsers(pcsId);

                var orgUsersKeyValuePairs =
                    orgUsers.Select(
                        ou =>
                            new KeyValuePair<string, Guid>(
                                ou.User.FirstName + " " + ou.User.Surname + " (" +
                                ou.UserStatus.ToString() + ")", ou.Id));

                model.OrganisationUsers = orgUsersKeyValuePairs.ToList();

                return View(model);
            }

            return RedirectToAction("ManageOrganisationUser", "Home",
                   new { area = "Scheme", pcsId, organisationUserId = model.SelectedOrganisationUser });
        }

        [HttpGet]
        public async Task<ActionResult> ManageOrganisationUser(Guid pcsId, Guid? organisationUserId)
        {
            if (organisationUserId.HasValue)
            {
                await SetBreadcrumb(pcsId, "Manage organisation users");

                using (var client = apiClient())
                {
                    var organisationExists =
                       await client.SendAsync(User.GetAccessToken(), new VerifyOrganisationExists(pcsId));

                    if (!organisationExists)
                    {
                        throw new ArgumentException("No organisation found for supplied organisation Id", "pcsId");
                    }

                    var orgUser = await client.SendAsync(User.GetAccessToken(), new GetOrganisationUser(organisationUserId.Value));
                    var model = new OrganisationUserViewModel(orgUser);
                    model.PossibleValues = GetUserPossibleStatusToBeChanged(model.UserStatus);
                    model.PossibleValues.Add(DoNotChange);
                    return View("ManageOrganisationUser", model);
                }
            }
            else
            {
                return RedirectToAction("ManageOrganisationUsers", "Home",
                        new { area = "Scheme", pcsId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageOrganisationUser(Guid pcsId, OrganisationUserViewModel model)
        {
            await SetBreadcrumb(pcsId, "Manage organisation users");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.SelectedValue != DoNotChange)
            {
                using (var client = apiClient())
                {
                    var userStatus = model.SelectedValue.GetValueFromDisplayName<UserStatus>();
                    await
                        client.SendAsync(User.GetAccessToken(), new UpdateOrganisationUserStatus(model.OrganisationUserId, userStatus));
                }
            }

            return RedirectToAction("ManageOrganisationUsers", "Home",
                   new { area = "Scheme", pcsId });
        }

        [HttpGet]
        public async Task<ActionResult> ViewOrganisationDetails(Guid pcsId)
        {
            using (var client = apiClient())
            {
                var organisationExists =
                    await client.SendAsync(User.GetAccessToken(), new VerifyOrganisationExists(pcsId));

                if (!organisationExists)
                {
                    throw new ArgumentException("No organisation found for supplied organisation Id", "pcsId");
                }

                var orgDetails = await client.SendAsync(User.GetAccessToken(), new GetOrganisationInfo(pcsId));

                var model = new ViewOrganisationDetailsViewModel
                {
                    OrganisationData = orgDetails
                };
                string organisationDetailsActivityName = orgDetails.OrganisationType == OrganisationType.RegisteredCompany ? PcsAction.ViewRegisteredOfficeDetails : PcsAction.ViewPrinciplePlaceOfBusinessDetails;
                await SetBreadcrumb(pcsId, organisationDetailsActivityName);
                return View("ViewOrganisationDetails", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewOrganisationDetails(Guid pcsId, ViewOrganisationDetailsViewModel model)
        {
            return RedirectToAction("ChooseActivity", "Home", new { pcsId });
        }

        [HttpGet]
        public async Task<ActionResult> ManageContactDetails(Guid pcsId)
        {
            await SetBreadcrumb(pcsId, "Manage organisation contact details");

            OrganisationData model;
            using (var client = apiClient())
            {
                model = await client.SendAsync(User.GetAccessToken(), new GetOrganisationInfo(pcsId));
                model.OrganisationAddress.Countries = await client.SendAsync(User.GetAccessToken(), new GetCountries(false));
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageContactDetails(OrganisationData model)
        {
            await SetBreadcrumb(model.Id, "Manage organisation contact details");

            if (!ModelState.IsValid)
            {
                using (var client = apiClient())
                {
                    model.OrganisationAddress.Countries = await client.SendAsync(User.GetAccessToken(), new GetCountries(false));
                }
                return View(model);
            }

            using (var client = apiClient())
            {
                await client.SendAsync(User.GetAccessToken(), new UpdateOrganisationContactDetails(model, true));
            }

            return RedirectToAction("ChooseActivity", new { pcsId = model.Id });
        }

        [HttpGet]
        public async Task<ActionResult> ViewSubmissionHistory(Guid pcsId,
            SubmissionsHistoryOrderBy orderBy = SubmissionsHistoryOrderBy.ComplianceYearDescending, int page = 1)
        {
            await SetBreadcrumb(pcsId, "View submission history");

            if (page < 1)
            {
                page = 1;
            }

            var model = new SubmissionHistoryViewModel();
            model.OrderBy = orderBy;

            using (var client = apiClient())
            {
                var scheme = await client.SendAsync(User.GetAccessToken(), new GetSchemePublicInfo(pcsId));

                if (scheme != null)
                {
                    var getSubmissionsHistoryResults =
                        new GetSubmissionsHistoryResults(scheme.SchemeId, scheme.OrganisationId,
                        ordering: orderBy);

                    var results = await client.SendAsync(User.GetAccessToken(), getSubmissionsHistoryResults);
                    if (results.Data != null)
                    {
                        model.Results = results.Data.ToPagedList(page - 1, DefaultPageSize, results.ResultCount);
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> DownloadCsv(Guid schemeId, int year, Guid memberUploadId, DateTime submissionDateTime)
        {
            using (var client = apiClient())
            {
                IEnumerable<ErrorData> errors =
                    (await client.SendAsync(User.GetAccessToken(), new GetMemberUploadData(schemeId, memberUploadId)))
                    .OrderByDescending(e => e.ErrorLevel);

                CsvWriter<ErrorData> csvWriter = csvWriterFactory.Create<ErrorData>();
                csvWriter.DefineColumn("Description", e => e.Description);

                var schemePublicInfo = await cache.FetchSchemePublicInfo(schemeId);
                var csvFileName = string.Format("{0}_memberregistration_{1}_warnings_{2}.csv", schemePublicInfo.ApprovalNo, year, submissionDateTime.ToString("ddMMyyyy_HHmm"));

                string csv = csvWriter.Write(errors);
                byte[] fileContent = new UTF8Encoding().GetBytes(csv);
                return File(fileContent, "text/csv", CsvFilenameFormat.FormatFileName(csvFileName));
            }
        }

        [HttpGet]
        public async Task<ActionResult> ViewDataReturnSubmissionHistory(Guid pcsId,
            DataReturnSubmissionsHistoryOrderBy orderBy = DataReturnSubmissionsHistoryOrderBy.ComplianceYearDescending, int page = 1)
        {
            await SetBreadcrumb(pcsId, "View submission history");

            if (page < 1)
            {
                page = 1;
            }

            var model = new DataReturnSubmissionHistoryViewModel();
            model.OrderBy = orderBy;

            using (var client = apiClient())
            {
                var scheme = await client.SendAsync(User.GetAccessToken(), new GetSchemePublicInfo(pcsId));

                if (scheme != null)
                {
                    var getDataReturnSubmissionsHistoryResults =
                        new GetDataReturnSubmissionsHistoryResults(scheme.SchemeId,
                        scheme.OrganisationId, ordering: orderBy);

                    var results = await client.SendAsync(User.GetAccessToken(), getDataReturnSubmissionsHistoryResults);
                    if (results.Data != null)
                    {
                        model.Results = results.Data.ToPagedList(page - 1, DefaultPageSize, results.ResultCount);
                    }
                }
            }

            return View(model);
        }

        private IList<string> GetUserPossibleStatusToBeChanged(UserStatus userStatus)
        {
            var userStatuses = new RadioButtonGenericStringCollectionViewModel<UserStatus>();

            switch (userStatus)
            {
                case UserStatus.Active:
                    {
                        return userStatuses.PossibleValues.Where(us => us.Equals(UserStatus.Inactive.ToString())).ToList();
                    }
                case UserStatus.Pending:
                    {
                        return userStatuses.PossibleValues.Where(us => us.Equals(UserStatus.Active.ToString()) || us.Equals(UserStatus.Rejected.ToString())).ToList();
                    }
                case UserStatus.Inactive:
                    {
                        return userStatuses.PossibleValues.Where(us => us.Equals(UserStatus.Active.ToString())).ToList();
                    }
                case UserStatus.Rejected:
                    {
                        return userStatuses.PossibleValues.Where(us => us.Equals(UserStatus.Active.ToString())).ToList();
                    }
            }
            return userStatuses.PossibleValues;
        }

        private async Task<List<OrganisationUserData>> GetOrganisationUsers(Guid pcsId)
        {
            using (var client = apiClient())
            {
                var orgUsers = await client.SendAsync(User.GetAccessToken(),
                    new GetManageableOrganisationUsers(pcsId));

                var loggedInUserId = User.GetUserId();

                return orgUsers.Where(ou => ou.UserId != loggedInUserId).ToList();
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