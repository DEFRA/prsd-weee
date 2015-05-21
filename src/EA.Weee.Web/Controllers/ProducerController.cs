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
    using Requests.Notification;
    using Requests.Producers;
    using Requests.Registration;
    using Requests.Shared;
    using ViewModels.NotificationApplication;
    using ViewModels.Shared;

    [Authorize]
    public class ProducerController : Controller
    {
        private readonly Func<IIwsClient> apiClient;

        public ProducerController(Func<IIwsClient> apiClient)
        {
            this.apiClient = apiClient;
        }

        [HttpGet]
        public ActionResult CopyFromExporter(Guid id)
        {
            var model = new YesNoChoiceViewModel();
            ViewBag.NotificationId = id;

            return View(model);
        }

        [HttpPost]
        public ActionResult CopyFromExporter(Guid id, YesNoChoiceViewModel inputModel)
        {
            var model = new ExporterNotifier();

            model.NotificationId = id;

            if (!ModelState.IsValid)
            {
                ViewBag.NotificationId = id;
                return View(inputModel);
            }

            if (inputModel.Choices.SelectedValue.Equals("No"))
            {
                return RedirectToAction("Add", new { id, copy = false });
            }

            return RedirectToAction("Add", new { id, copy = true });
        }

        [HttpGet]
        public async Task<ActionResult> Add(Guid id, bool? copy)
        {
            var model = new ProducerData
            {
                Address = new AddressData(),
                Business = new BusinessData()
            };

            if (copy.HasValue && copy.Value)
            {
                using (var client = apiClient())
                {
                    var exporter = await client.SendAsync(new GetExporterByNotificationId(id));

                    model.Address = exporter.Address;
                    model.Contact = exporter.Contact;
                    model.Business = new BusinessData
                    {
                        Name = exporter.Name,
                        EntityType = exporter.Type,
                        AdditionalRegistrationNumber = exporter.AdditionalRegistrationNumber
                    };
                    model.Business.BindRegistrationNumber(exporter.RegistrationNumber);
                }
            }

            model.NotificationId = id;

            await BindCountrySelectList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(ProducerData model)
        {
            if (!ModelState.IsValid)
            {
                await BindCountrySelectList();
                return View(model);
            }

            using (var client = apiClient())
            {
                try
                {
                    var response = await client.SendAsync(User.GetAccessToken(), new AddProducerToNotification(model));

                    return RedirectToAction("MultipleProducers", "Producer",
                        new { notificationId = model.NotificationId });
                }
                catch (ApiBadRequestException ex)
                {
                    this.HandleBadRequest(ex);

                    if (ModelState.IsValid)
                    {
                        throw;
                    }
                }

                await BindCountrySelectList(client);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> MultipleProducers(Guid notificationId)
        {
            var model = new MultipleProducersViewModel();

            using (var client = apiClient())
            {
                var response =
                    await client.SendAsync(User.GetAccessToken(), new GetProducersByNotificationId(notificationId));

                try
                {
                    model.NotificationId = notificationId;
                    model.ProducerData = response.ToList();
                    model.HasSiteOfExport = model.ProducerData.Exists(p => p.IsSiteOfExport);
                    return View("MultipleProducers", model);
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
        }

        private async Task BindCountrySelectList()
        {
            using (var client = apiClient())
            {
                await BindCountrySelectList(client);
            }
        }

        private async Task BindCountrySelectList(IIwsClient client)
        {
            var response = await client.SendAsync(new GetCountries());

            ViewBag.Countries = new SelectList(response, "Id", "Name",
                response.Single(c => c.Name.Equals("United Kingdom", StringComparison.InvariantCultureIgnoreCase)).Id);
        }
    }
}