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
    using Weee.Requests.Users;

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

            IEnumerable<OrganisationUserData> accessibleOrganisations = organisations
                .Where(o => o.UserStatus == UserStatus.Approved);

            IEnumerable<OrganisationUserData> inaccessibleOrganisations = organisations
                .Where(o => o.UserStatus != UserStatus.Approved);

            if (accessibleOrganisations.Any())
            {
                YourOrganisationsViewModel model = new YourOrganisationsViewModel();
                
                foreach (OrganisationUserData organisation in accessibleOrganisations)
                {
                    RadioButtonPair<string, Guid> item = new RadioButtonPair<string, Guid>(
                        organisation.Organisation.Name,
                        organisation.OrganisationId);

                    model.AccessibleOrganisations.PossibleValues.Add(item);
                }

                ViewBag.InaccessibleOrganisations = inaccessibleOrganisations;
                return View("YourOrganisations", model);
            }
            else if (inaccessibleOrganisations.Any())
            {
                PendingOrganisationsViewModel model = new PendingOrganisationsViewModel();

                model.InaccessibleOrganisations = inaccessibleOrganisations;

                return View("PendingOrganisations", model);
            }
            else
            {
                return RedirectToAction("Type", "OrganisationRegistration");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(YourOrganisationsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("YourOrganisations", model);
            }

            Guid pcsId = model.AccessibleOrganisations.SelectedValue;

            return RedirectToAction("ChooseActivity", "Home", new { area = "Scheme", pcsId });
        }

        [ChildActionOnly]
        public ActionResult _Pending(bool alreadyLoaded, IEnumerable<OrganisationUserData> inaccessibleOrganisations)
        {
            if (!alreadyLoaded)
            {
                var task = Task.Run(async () => { return await GetOrganisations(); });
                task.Wait();

                IEnumerable<OrganisationUserData> organisations = task.Result;

                inaccessibleOrganisations = organisations
                    .Where(o => o.UserStatus != UserStatus.Approved);
            }

            return PartialView(inaccessibleOrganisations);
        }

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

            return organisations.Where(o => o.Organisation.OrganisationStatus == OrganisationStatus.Complete);
        }
    }
}