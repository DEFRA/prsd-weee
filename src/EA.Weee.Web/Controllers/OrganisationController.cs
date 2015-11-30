namespace EA.Weee.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Base;
    using Core.Organisations;
    using Core.Shared;
    using Infrastructure;
    using ViewModels.Organisation;
    using ViewModels.Shared;
    using Weee.Requests.Organisations;

    public class OrganisationController : ExternalSiteController
    {
        private readonly Func<IWeeeClient> apiClient;

        public OrganisationController(Func<IWeeeClient> apiClient)
        {
            this.apiClient = apiClient;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            IEnumerable<OrganisationUserData> organisations = await GetOrganisations();

            List<OrganisationUserData> accessibleOrganisations = organisations
                .Where(o => o.UserStatus == UserStatus.Active)
                .ToList();

            List<OrganisationUserData> inaccessibleOrganisations = organisations
                .Except(accessibleOrganisations)
                .ToList();

            if (accessibleOrganisations.Count == 1 && inaccessibleOrganisations.Count == 0)
            {
                Guid organisationId = accessibleOrganisations[0].OrganisationId;
                return RedirectToAction("ChooseActivity", "Home", new { area = "Scheme", pcsId = organisationId });
            }
            
            if (accessibleOrganisations.Count > 0)
            {
                YourOrganisationsViewModel model = new YourOrganisationsViewModel();
                
                foreach (OrganisationUserData organisation in accessibleOrganisations)
                {
                    RadioButtonPair<string, Guid> item = new RadioButtonPair<string, Guid>(
                        organisation.Organisation.OrganisationName,
                        organisation.OrganisationId);

                    model.AccessibleOrganisations.PossibleValues.Add(item);
                }

                ViewBag.InaccessibleOrganisations = inaccessibleOrganisations.Where(o => o.UserStatus == UserStatus.Pending);
                return View("YourOrganisations", model);
            }
            
            if (inaccessibleOrganisations.Count > 0)
            {
                PendingOrganisationsViewModel model = new PendingOrganisationsViewModel();

                model.InaccessibleOrganisations = inaccessibleOrganisations;

                return View("PendingOrganisations", model);
            }

            return RedirectToAction("Search", "OrganisationRegistration");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(YourOrganisationsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("YourOrganisations", model);
            }

            Guid organisationId = model.AccessibleOrganisations.SelectedValue;

            return RedirectToAction("ChooseActivity", "Home", new { area = "Scheme", pcsId = organisationId });
        }

        [ChildActionOnly]
        public ActionResult _Pending(bool alreadyLoaded, IEnumerable<OrganisationUserData> inaccessibleOrganisations)
        {
            if (!alreadyLoaded)
            {
                // MVC 5 doesn't allow child actions to run asynchornously, so we
                // have to schedule this task and block the calling thread.
                var task = Task.Run(async () => { return await GetOrganisations(); });
                task.Wait();

                IEnumerable<OrganisationUserData> organisations = task.Result;

                inaccessibleOrganisations = organisations
                    .Where(o => o.UserStatus == UserStatus.Pending);
            }

            return PartialView(inaccessibleOrganisations);
        }

        /// <summary>
        /// Returns all complete organisations with which the user is associated,
        /// ordered by organisation name.
        /// </summary>
        /// <returns></returns>
        private async Task<IEnumerable<OrganisationUserData>> GetOrganisations()
        {
            List<OrganisationUserData> organisations;

            using (var client = apiClient())
            {
                organisations = await
                 client.SendAsync(
                     User.GetAccessToken(),
                     new GetUserOrganisationsByStatus(new int[0]));
            }

            return organisations
                .Where(o => o.Organisation.OrganisationStatus == OrganisationStatus.Complete)
                .OrderBy(o => o.Organisation.OrganisationName);
        }
    }
}