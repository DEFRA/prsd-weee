namespace EA.Weee.Web.Areas.Scheme.Controllers
{
    using Api.Client;
    using Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests;
    using EA.Weee.Requests.Scheme;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using Infrastructure;
    using Prsd.Core.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using ViewModels;
    using Web.Controllers.Base;
    using Web.ViewModels.Shared;
    using Weee.Requests.Organisations;
    using Weee.Requests.Users;
    using Weee.Requests.Users.GetManageableOrganisationUsers;

    [Authorize]
    public class HomeController : ExternalSiteController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IWeeeCache cache;
        private readonly BreadcrumbService breadcrumb;
        private const string DoNotChange = "Do not change at this time";

        public HomeController(Func<IWeeeClient> apiClient, IWeeeCache cache, BreadcrumbService breadcrumb)
        {
            this.apiClient = apiClient;
            this.cache = cache;
            this.breadcrumb = breadcrumb;
        }

        [HttpGet]
        public async Task<ActionResult> ChooseActivity(Guid pcsId)
        {
            var model = new ChooseActivityViewModel();
            using (var client = apiClient())
            {
                var organisationExists =
                    await client.SendAsync(User.GetAccessToken(), new VerifyOrganisationExists(pcsId));

                if (!organisationExists)
                {
                    throw new ArgumentException("No organisation found for supplied organisation Id", "organisationId");
                }

                var orgUsers = await GetOrganisationUsers(pcsId);

                if (orgUsers.Count == 0)
                {
                    model.ActivityOptions.PossibleValues.Remove(PcsAction.ManageOrganisationUsers);
                }

                model.OrganisationId = pcsId;
                await SetBreadcrumb(pcsId, null);

                await SetShowLinkToCreateOrJoinOrganisation(model);

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChooseActivity(ChooseActivityViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.ActivityOptions.SelectedValue == PcsAction.ManagePcsMembers)
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
                if (viewModel.ActivityOptions.SelectedValue == PcsAction.ManageOrganisationUsers)
                {
                    return RedirectToAction("ManageOrganisationUsers", new { pcsId = viewModel.OrganisationId });
                }
                if (viewModel.ActivityOptions.SelectedValue == PcsAction.ViewOrganisationDetails)
                {
                    return RedirectToAction("ViewOrganisationDetails", new { pcsId = viewModel.OrganisationId });
                }
                if (viewModel.ActivityOptions.SelectedValue == PcsAction.ManageContactDetails)
                {
                    return RedirectToAction("ManageContactDetails", new { pcsId = viewModel.OrganisationId });
                }
            }

            await SetBreadcrumb(viewModel.OrganisationId, null);
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
            var model = new OrganisationUsersViewModel();

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

                var orgUserRadioButtons = new StringGuidRadioButtons(orgUsersKeyValuePairs);
                model.OrganisationUsers = orgUserRadioButtons;
            }

            await SetBreadcrumb(pcsId, "Manage users");
            return View("ManageOrganisationUsers", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageOrganisationUsers(Guid pcsId, OrganisationUsersViewModel model)
        {
            await SetBreadcrumb(pcsId, "Manage users");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            return RedirectToAction("ManageOrganisationUser", "Home",
                   new { area = "Scheme", pcsId, organisationUserId = model.OrganisationUsers.SelectedValue });
        }

        [HttpGet]
        public async Task<ActionResult> ManageOrganisationUser(Guid pcsId, Guid organisationUserId)
        {
            await SetBreadcrumb(pcsId, "Manage users");

            using (var client = apiClient())
            {
                var organisationExists =
                   await client.SendAsync(User.GetAccessToken(), new VerifyOrganisationExists(pcsId));

                if (!organisationExists)
                {
                    throw new ArgumentException("No organisation found for supplied organisation Id", "pcsId");
                }

                var orgUser = await client.SendAsync(User.GetAccessToken(), new GetOrganisationUser(organisationUserId));
                var model = new OrganisationUserViewModel(orgUser);
                model.UserStatuses = GetUserPossibleStatusToBeChanged(model.UserStatuses, model.UserStatus);
                model.UserStatuses.PossibleValues.Add(DoNotChange);
                return View("ManageOrganisationUser", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageOrganisationUser(Guid pcsId, OrganisationUserViewModel model)
        {
            await SetBreadcrumb(pcsId, "Manage users");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.UserStatuses.SelectedValue != DoNotChange)
            {
                using (var client = apiClient())
                {
                    var userStatus = model.UserStatuses.SelectedValue.GetValueFromDisplayName<UserStatus>();
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
            await SetBreadcrumb(pcsId, "Organisation details");

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
            await SetBreadcrumb(pcsId, "Organisation contact details");

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
            await SetBreadcrumb(model.Id, "Organisation contact details");

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

        private RadioButtonStringCollectionViewModel GetUserPossibleStatusToBeChanged(RadioButtonStringCollectionViewModel userStatuses, UserStatus userStatus)
        {
            switch (userStatus)
            {
                case UserStatus.Active:
                    {
                        var possibleValues = userStatuses.PossibleValues.Where(us => us.Equals(UserStatus.Inactive.ToString())).ToList();
                        userStatuses.PossibleValues = possibleValues;
                        return userStatuses;
                    }
                case UserStatus.Pending:
                    {
                        var possibleValues = userStatuses.PossibleValues.Where(us => us.Equals(UserStatus.Active.ToString()) || us.Equals(UserStatus.Rejected.ToString())).ToList();
                        userStatuses.PossibleValues = possibleValues;
                        return userStatuses;
                    }
                case UserStatus.Inactive:
                    {
                        var possibleValues = userStatuses.PossibleValues.Where(us => us.Equals(UserStatus.Active.ToString())).ToList();
                        userStatuses.PossibleValues = possibleValues;
                        return userStatuses;
                    }
                case UserStatus.Rejected:
                    {
                        var possibleValues = userStatuses.PossibleValues.Where(us => us.Equals(UserStatus.Active.ToString())).ToList();
                        userStatuses.PossibleValues = possibleValues;
                        return userStatuses;
                    }
            }
            return userStatuses;
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
    }
}