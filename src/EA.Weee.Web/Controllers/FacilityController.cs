namespace EA.Weee.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Infrastructure;
    using Prsd.Core.Web.ApiClient;
    using Prsd.Core.Web.Mvc.Extensions;
    using Requests.Facilities;
    using Requests.Registration;
    using Requests.Shared;
    using ViewModels.NotificationApplication;

    [Authorize]
    public class FacilityController : Controller
    {
        private readonly Func<IIwsClient> apiClient;

        public FacilityController(Func<IIwsClient> apiClient)
        {
            this.apiClient = apiClient;
        }

        [HttpGet]
        public async Task<ActionResult> Add(Guid id)
        {
            NotificationType notificationType;

            using (var client = apiClient())
            {
                await BindCountrySelectList(client);

                notificationType = await client.SendAsync(new NotificationTypeByNotificationId(id));
            }

            var facility = new FacilityData
            {
                NotificationId = id,
                NotificationType = notificationType
            };

            return View(facility);
        }

        [HttpPost]
        public async Task<ActionResult> Add(FacilityData model)
        {
            try
            {
                using (var client = apiClient())
                {
                    var response = await client.SendAsync(User.GetAccessToken(), new AddFacilityToNotification(model));

                    return RedirectToAction("MultipleFacilities", "Facility",
                        new { notificationID = model.NotificationId });
                }
            }
            catch (ApiBadRequestException ex)
            {
                this.HandleBadRequest(ex);

                if (ModelState.IsValid)
                {
                    throw;
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> MultipleFacilities(Guid notificationId, string errorMessage = "")
        {
            var model = new MultipleFacilitiesViewModel();

            if (!String.IsNullOrEmpty(errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
            }

            using (var client = apiClient())
            {
                var response =
                    await client.SendAsync(User.GetAccessToken(), new GetFacilitiesByNotificationId(notificationId));

                model.NotificationId = notificationId;
                model.FacilityData = response.ToList();
            }

            return View("MultipleFacilities", model);
        }

        [HttpPost]
        public ActionResult MultipleFacilities(MultipleFacilitiesViewModel model)
        {
            return RedirectToAction("Index", "Home");
        }

        private async Task BindCountrySelectList(IIwsClient client)
        {
            var response = await client.SendAsync(new GetCountries());

            ViewBag.Countries = new SelectList(response, "Id", "Name",
                response.Single(c => c.Name.Equals("United Kingdom", StringComparison.InvariantCultureIgnoreCase)).Id);
        }
    }
}