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

        private async Task<List<string>> GetActivities(Guid pcsId)
        {
            using (var client = apiClient())
            {
                var organisationDetails = await client.SendAsync(User.GetAccessToken(), new GetOrganisationInfo(pcsId));
                //get the organisation type based on organisation id
                string organisationDetailsActivityName = organisationDetails.OrganisationType == OrganisationType.RegisteredCompany ? PcsAction.ViewRegisteredOfficeDetails : PcsAction.ViewPrinciplePlaceOfBusinessDetails;

                var organisationOverview = await client.SendAsync(User.GetAccessToken(), new GetOrganisationOverview(pcsId));

                List<string> activities = new List<string>();
                activities.Add(PcsAction.ManagePcsMembers);
                if (configurationService.CurrentConfiguration.EnableDataReturns)
                {
                    activities.Add(PcsAction.SubmitPcsDataReturns);
                }
                if (organisationOverview.HasMemberSubmissions)
                {
                    activities.Add(PcsAction.ViewSubmissionHistory);
                }
                activities.Add(organisationDetailsActivityName);
                activities.Add(PcsAction.ManageContactDetails);
                if (organisationOverview.HasMultipleOrganisationUsers)
                {
                    activities.Add(PcsAction.ManageOrganisationUsers);
                }

                return activities;
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
                    return RedirectToAction("ViewSubmissionHistory", new { pcsId = viewModel.OrganisationId });
                }
                if (viewModel.SelectedValue == PcsAction.SubmitPcsDataReturns)
                {
                    using (var client = apiClient())
                    {
                        var status = await client.SendAsync(User.GetAccessToken(), new GetSchemeStatus(viewModel.OrganisationId));

                        if (status == SchemeStatus.Approved)
                        {
                            return RedirectToAction("SubmitDataReturns", "DataReturns", new { pcsId = viewModel.OrganisationId });
                        }
                        else
                        {
                            return RedirectToAction("AuthorisationRequired", "MemberRegistration", new { pcsId = viewModel.OrganisationId });
                        }
                    }
                }
            }

            await SetBreadcrumb(viewModel.OrganisationId, null);
            viewModel.PossibleValues = await GetActivities(viewModel.OrganisationId);
            await SetShowLinkToCreateOrJoinOrganisation(viewModel);
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
                await client.SendAsync(User.GetAccessToken(), new UpdateOrganisationContactDetails(model));
            }

            return RedirectToAction("ChooseActivity", new { pcsId = model.Id });
        }

        [HttpGet]
        public async Task<ActionResult> ViewSubmissionHistory(Guid pcsId)
        {
            await SetBreadcrumbAndPcsBanner(pcsId, "View submission history");

            var model = new SubmissionHistoryViewModel();
            
            using (var client = apiClient())
            {
                var scheme = await client.SendAsync(User.GetAccessToken(), new GetSchemePublicInfo(pcsId));

                if (scheme != null)
                {
                    model.Results = await client.SendAsync(User.GetAccessToken(), new GetSubmissionsHistoryResults(scheme.SchemeId, scheme.OrganisationId));
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> DownloadCsv(Guid schemeId, int year, Guid memberUploadId, DateTime submissionDateTime)
        {
            using (var client = apiClient())
            {
                IEnumerable<UploadErrorData> errors =
                    (await client.SendAsync(User.GetAccessToken(), new GetMemberUploadData(schemeId, memberUploadId)))
                    .OrderByDescending(e => e.ErrorLevel);

                CsvWriter<UploadErrorData> csvWriter = csvWriterFactory.Create<UploadErrorData>();
                csvWriter.DefineColumn("Description", e => e.Description);
    
                var schemePublicInfo = await cache.FetchSchemePublicInfo(schemeId);
                var csvFileName = string.Format("{0}_memberregistration_{1}_warnings_{2}.csv", schemePublicInfo.ApprovalNo, year, submissionDateTime.ToString("ddMMyyyy_HHmm"));

                string csv = csvWriter.Write(errors);
                byte[] fileContent = new UTF8Encoding().GetBytes(csv);
                return File(fileContent, "text/csv", CsvFilenameFormat.FormatFileName(csvFileName));
            }
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
        }

        private async Task SetBreadcrumbAndPcsBanner(Guid organisationId, string activity)
        {
            await SetBreadcrumb(organisationId, activity);
            breadcrumb.SchemeInfo = await cache.FetchSchemePublicInfo(organisationId);
        }
    }
}