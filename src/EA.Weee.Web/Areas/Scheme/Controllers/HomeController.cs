namespace EA.Weee.Web.Areas.Scheme.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.Scheme;
    using Infrastructure;
    using ViewModels;
    using Web.Controllers.Base;
    using Web.ViewModels.Shared;
    using Weee.Requests.Organisations;
    using Weee.Requests.Users;

    [Authorize]
    public class HomeController : ExternalSiteController
    {
        private readonly Func<IWeeeClient> apiClient;

        public HomeController(Func<IWeeeClient> apiClient)
        {
            this.apiClient = apiClient;
        }

        // GET: Scheme/Home
        public ActionResult Index()
        {
            return View();
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

                model.OrganisationId = pcsId;
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
                            return RedirectToAction("AuthorizationRequired", "MemberRegistration", new { pcsId = viewModel.OrganisationId });
                        }
                    }
                }
                if (viewModel.ActivityOptions.SelectedValue == PcsAction.ManageOrganisationUsers)
                {
                    return RedirectToAction("ManageOrganisationUsers", new { pcsId = viewModel.OrganisationId });
                }
            }

            return View(viewModel);
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

                var orgUsers = await client.SendAsync(User.GetAccessToken(),
                    new GetUsersByOrganisationId(pcsId));

                var loggedInUserId = User.GetUserId();

                orgUsers = orgUsers.Where(ou => ou.UserId != loggedInUserId).ToList();

                var orgUsersKeyValuePairs =
                    orgUsers.Select(
                        ou =>
                            new KeyValuePair<string, Guid>(
                                ou.User.FirstName + " " + ou.User.Surname + " - (" +
                                ou.UserStatus.ToString() + ")", new Guid(ou.UserId)));

                var orgUserRadioButtons = new StringGuidRadioButtons(orgUsersKeyValuePairs);
                model.OrganisationUsers = orgUserRadioButtons;
            }
            return View("ManageOrganisationUsers", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageOrganisationUsers(OrganisationUsersViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            return View(model);
        }
    }
}