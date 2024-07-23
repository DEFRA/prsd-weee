namespace EA.Weee.Web.Controllers
{
    using Api.Client;
    using Base;
    using Core.Organisations;
    using Core.Shared;
    using Infrastructure;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Org.BouncyCastle.Utilities;
    using RestSharp;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using System.Web.UI.WebControls;
    using ViewModels.Organisation;
    using Weee.Requests.Organisations;
    using static Google.Apis.Requests.BatchRequest;

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

            return RedirectToAction("ChooseActivity", "Home", new { area = "Scheme", pcsId = model.SelectedOrganisationId.Value });
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

                ViewBag.InaccessibleOrganisations = inaccessibleOrganisations.Where(o => o.UserStatus == UserStatus.Pending);
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
        private IEnumerable<OrganisationUserData> FilterOutDuplicateOrganisations(IEnumerable<OrganisationUserData> organisations)
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
                    bool alreadyAdded = results
                        .Where(r => r.OrganisationId == organistion.OrganisationId)
                        .Where(r => r.UserId == organistion.UserId)
                        .Any();

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
        [AllowAnonymous]
        public ActionResult Companies()
        {
            return View(new CompaniesHouseModel());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Companies(CompaniesHouseModel model)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(model.RegistrationNumber))
                {
                    if (model.UseDefra)
                    {
                        string filePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + @"\Cert\Boomi-IWS-TST.pfx";

                        X509Certificate2 certificate = new X509Certificate2(filePath, "kN2S6!p6F*LH");
                        HttpClientHandler handler = new HttpClientHandler();

                        handler.ClientCertificates.Add(certificate);

                        HttpClient client = new HttpClient(handler);

                        string requestUrl = $"https://integration-tst.azure.defra.cloud/ws/rest/DEFRA/v2.1/CompaniesHouse/companies/{model.RegistrationNumber}";

                        HttpResponseMessage response = await client.GetAsync(requestUrl);

                        if (response.IsSuccessStatusCode)
                        {
                            string content = await response.Content.ReadAsStringAsync();

                            DefraCompaniesHouseApiModel companyModel = JsonConvert.DeserializeObject<DefraCompaniesHouseApiModel>(content);
                            companyModel.RegistrationNumber = model.RegistrationNumber;
                            model.Response = JToken.Parse(content).ToString(Newtonsoft.Json.Formatting.Indented);
                            model.DefraCompaniesHouseApiModel = companyModel;

                            return View(model);
                        }
                        else
                        {
                            model.Error = response.StatusCode.ToString();

                            return View(model);
                        }
                    }
                    else
                    {
                        RestClient client = new RestClient("https://api.company-information.service.gov.uk");
                        var request = new RestRequest($"/company/{model.RegistrationNumber}", Method.Get);
                        byte[] uuidBytes = Encoding.UTF8.GetBytes("2356a6d8-8a0a-4acb-9165-68a77cc81755");
                        string base64Uuid = Convert.ToBase64String(uuidBytes);
                        request.AddHeader("Authorization", $"Basic {base64Uuid}");

                        var response = await client.ExecuteAsync<String>(request);

                        if (!response.IsSuccessStatusCode)
                        {
                            model.Error = response.StatusCode.ToString();

                            return View(model);
                        }

                        CompaniesHouseApiModel companiesHouseApiModel = JsonConvert.DeserializeObject<CompaniesHouseApiModel>(response.Content);
                        model.Response = JToken.Parse(response.Content).ToString(Newtonsoft.Json.Formatting.Indented);
                        model.CompaniesHouseApiModel = companiesHouseApiModel;

                        return View(model);
                    }
                }
              
                return View(model);
            }
            catch (Exception ex)
            {
                model.Error = ex.Message;

                return View(model);
            }
        }
    }
}