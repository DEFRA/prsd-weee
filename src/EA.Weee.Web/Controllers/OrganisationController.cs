namespace EA.Weee.Web.Controllers
{
    using Api.Client;
    using Base;
    using Core.Organisations;
    using Core.Shared;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Web.ViewModels.Organisation.Mapping.ToViewModel;
    using Infrastructure;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using ViewModels.Organisation;
    using Weee.Requests.Organisations;

    public class OrganisationController : ExternalSiteController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IMapper mapper;

        public OrganisationController(Func<IWeeeClient> apiClient, IMapper mapper)
        {
            this.apiClient = apiClient;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            return await ShowOrganisations();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(YourOrganisationsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return await ShowOrganisations();
            }

            return RedirectToAction("ChooseActivity", "Home",
                new { area = "Scheme", pcsId = model.SelectedOrganisationId.Value });
        }

        private async Task<ActionResult> ShowOrganisations()
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
                model.Organisations = accessibleOrganisations;

                ViewBag.InaccessibleOrganisations =
                    inaccessibleOrganisations.Where(o => o.UserStatus == UserStatus.Pending);
                return View("YourOrganisations", model);
            }

            if (inaccessibleOrganisations.Count > 0)
            {
                PendingOrganisationsViewModel model = new PendingOrganisationsViewModel();

                model.InaccessibleOrganisations = FilterOutDuplicateOrganisations(inaccessibleOrganisations);

                return View("PendingOrganisations", model);
            }

            return RedirectToAction("Search", "OrganisationRegistration");
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

        /// <summary>
        /// Where a user has multiple associations with an organisation, only the current association
        /// should be displayed. The order of perference is "Active", "Inactive", "Pending", "Rejected".
        /// </summary>
        /// <param name="organisations"></param>
        /// <returns></returns>
        private IEnumerable<OrganisationUserData> FilterOutDuplicateOrganisations(
            IEnumerable<OrganisationUserData> organisations)
        {
            List<UserStatus> userStatuesInOrderOfPreference = new List<UserStatus>()
            {
                UserStatus.Active,
                UserStatus.Inactive,
                UserStatus.Pending,
                UserStatus.Rejected
            };

            List<OrganisationUserData> results = new List<OrganisationUserData>();

            foreach (UserStatus userStatus in userStatuesInOrderOfPreference)
            {
                var organisationsWithMatchingUserStatus = organisations.Where(o => o.UserStatus == userStatus);

                foreach (OrganisationUserData organistion in organisationsWithMatchingUserStatus)
                {
                    var alreadyAdded = results
                        .Where(r => r.OrganisationId == organistion.OrganisationId)
                        .Any(r => r.UserId == organistion.UserId);

                    if (!alreadyAdded)
                    {
                        results.Add(organistion);
                    }
                }
            }

            // Now return the results in the same order that they were supplied to this method.
            foreach (OrganisationUserData organisation in organisations)
            {
                if (results.Contains(organisation))
                {
                    yield return organisation;
                }
            }
        }

        [HttpGet]
        public async Task<ActionResult> RepresentingOrganisation(Guid organisationId)
        {
            using (var client = apiClient())
            {
                var model = new RepresentingCompanyDetailsViewModel()
                {
                    OrganisationId = organisationId
                };

                var countries = await client.SendAsync(User.GetAccessToken(), new GetCountries(false));
                model.Address.Countries = countries;

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RepresentingOrganisation(RepresentingCompanyDetailsViewModel model)
        {
            using (var client = apiClient())
            {
                if (ModelState.IsValid)
                {
                    var result = await client.SendAsync(User.GetAccessToken(),
                        new AddRepresentingCompany(model.OrganisationId, model));

                    return RedirectToAction(nameof(Areas.Scheme.Controllers.HomeController.ChooseActivity),
                        typeof(Areas.Scheme.Controllers.HomeController).GetControllerName(),
                        new { pcsId = model.OrganisationId, directRegistrantId = result, area = "Scheme" });
                }

                var countries = await client.SendAsync(User.GetAccessToken(), new GetCountries(false));
                model.Address.Countries = countries;

                return View(model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> RepresentingCompanies(Guid organisationId)
        {
            using (var client = apiClient())
            {
                var organisationInfo =
                    await client.SendAsync(User.GetAccessToken(), new GetOrganisationInfo(organisationId));

                var organisations = await GetOrganisations();

                var model = mapper.Map<RepresentingCompaniesViewModelMapSource, RepresentingCompaniesViewModel>(new RepresentingCompaniesViewModelMapSource(organisations.ToList(), organisationInfo));

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RepresentingCompanies(RepresentingCompaniesViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.SelectedDirectRegistrant != null)
                {
                    return RedirectToAction(nameof(Areas.Scheme.Controllers.HomeController.ChooseActivity),
                        typeof(Areas.Scheme.Controllers.HomeController).GetControllerName(),
                        new { area = "Scheme", pcsId = model.OrganisationId, directRegistrantId = model.SelectedDirectRegistrant.Value });
                }
            }

            return View(model);
        }
    }
}