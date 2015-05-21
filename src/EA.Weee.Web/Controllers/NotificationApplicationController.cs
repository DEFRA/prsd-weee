namespace EA.Weee.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Infrastructure;
    using Prsd.Core;
    using Prsd.Core.Extensions;
    using Prsd.Core.Web.ApiClient;
    using Prsd.Core.Web.Mvc.Extensions;
    using Requests.Notification;
    using Requests.Registration;
    using Requests.Shared;
    using ViewModels.NotificationApplication;
    using ViewModels.Shared;
    using Constants = Prsd.Core.Web.Constants;

    [Authorize]
    public class NotificationApplicationController : Controller
    {
        private readonly Func<IIwsClient> apiClient;

        public NotificationApplicationController(Func<IIwsClient> apiClient)
        {
            this.apiClient = apiClient;
        }

        [HttpGet]
        public ActionResult CompetentAuthority()
        {
            var model = new CompetentAuthorityChoiceViewModel
            {
                CompetentAuthorities =
                    RadioButtonStringCollectionViewModel.CreateFromEnum<CompetentAuthority>()
            };

            return View("CompetentAuthority", model);
        }

        [HttpPost]
        public ActionResult CompetentAuthority(CompetentAuthorityChoiceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("CompetentAuthority", model);
            }

            return RedirectToAction("NotificationTypeQuestion",
                new { ca = model.CompetentAuthorities.SelectedValue });
        }

        [HttpGet]
        public ActionResult NotificationTypeQuestion(string ca, string nt)
        {
            var model = new InitialQuestionsViewModel
            {
                SelectedNotificationType = NotificationType.Recovery,
                CompetentAuthority = ca.GetValueFromDisplayName<CompetentAuthority>()
            };

            if (!string.IsNullOrWhiteSpace(nt))
            {
                model.SelectedNotificationType = nt.GetValueFromDisplayName<NotificationType>();
            }

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> NotificationTypeQuestion(InitialQuestionsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var client = apiClient())
            {
                try
                {
                    var response =
                        await
                            client.SendAsync(User.GetAccessToken(),
                                new CreateNotificationApplication
                                {
                                    CompetentAuthority = model.CompetentAuthority,
                                    NotificationType = model.SelectedNotificationType
                                });

                    return RedirectToAction("Created",
                        new
                        {
                            id = response
                        });
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

        [HttpGet]
        public async Task<ActionResult> Created(Guid id)
        {
            using (var client = apiClient())
            {
                var response = await client.SendAsync(User.GetAccessToken(), new GetNotificationNumber(id));

                var model = new CreatedViewModel
                {
                    NotificationId = id,
                    NotificationNumber = response
                };
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult Created(CreatedViewModel model)
        {
            return RedirectToAction("ExporterNotifier", "NotificationApplication", new { id = model.NotificationId });
        }

        [HttpGet]
        public async Task<ActionResult> GenerateNotificationDocument(Guid id)
        {
            using (var client = apiClient())
            {
                try
                {
                    var response =
                        await client.SendAsync(User.GetAccessToken(), new GenerateNotificationDocument(id));

                    var downloadName = "IwsNotification" + SystemTime.UtcNow + ".docx";

                    return File(response, Constants.MicrosoftWordContentType, downloadName);
                }
                catch (ApiBadRequestException ex)
                {
                    this.HandleBadRequest(ex);

                    if (ModelState.IsValid)
                    {
                        throw;
                    }
                }

                return HttpNotFound();
            }
        }

        private async Task<IEnumerable<CountryData>> GetCountries()
        {
            using (var client = apiClient())
            {
                return await client.SendAsync(new GetCountries());
            }
        }

        private async Task<IEnumerable<CountryData>> GetCountries(IIwsClient iwsClient)
        {
            return await iwsClient.SendAsync(new GetCountries());
        }

        [HttpGet]
        public async Task<ActionResult> ExporterNotifier(Guid id)
        {
            var model = new ExporterNotifier();
            var address = new AddressViewModel { Countries = await GetCountries() };
            var business = new BusinessViewModel();

            model.NotificationId = id;
            model.AddressDetails = address;
            model.BusinessViewModel = business;

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> ExporterNotifier(ExporterNotifier model)
        {
            if (!ModelState.IsValid)
            {
                model.AddressDetails.Countries = await GetCountries();
                return View(model);
            }

            using (var client = apiClient())
            {
                try
                {
                    var response = await client.SendAsync(User.GetAccessToken(), new CreateExporter
                    {
                        Name = model.BusinessViewModel.Name,
                        Type = model.BusinessViewModel.EntityType,
                        RegistrationNumber =
                            model.BusinessViewModel.CompaniesHouseRegistrationNumber ??
                            (model.BusinessViewModel.SoleTraderRegistrationNumber ??
                             model.BusinessViewModel.PartnershipRegistrationNumber),
                        AdditionalRegistrationNumber = model.BusinessViewModel.AdditionalRegistrationNumber,
                        NotificationId = model.NotificationId,
                        Building = model.AddressDetails.Building,
                        Address1 = model.AddressDetails.Address1,
                        Address2 = model.AddressDetails.Address2,
                        City = model.AddressDetails.TownOrCity,
                        County = model.AddressDetails.County,
                        PostCode = model.AddressDetails.Postcode,
                        CountryId = model.AddressDetails.CountryId,
                        FirstName = model.ContactDetails.FirstName,
                        LastName = model.ContactDetails.LastName,
                        Phone = model.ContactDetails.Telephone,
                        Email = model.ContactDetails.Email,
                        Fax = model.ContactDetails.Fax
                    });

                    return RedirectToAction("CopyFromExporter", "Producer", new { id = model.NotificationId });
                }
                catch (ApiBadRequestException ex)
                {
                    this.HandleBadRequest(ex);

                    if (ModelState.IsValid)
                    {
                        throw;
                    }
                }

                model.AddressDetails.Countries = await GetCountries(client);
                return View(model);
            }
        }

        public ActionResult _GetUserNotifications()
        {
            using (var client = apiClient())
            {
                // Child actions (partial views) cannot be async and we must therefore get the result of the task.
                // The called code must use ConfigureAwait(false) on async tasks to prevent deadlock.
                var response =
                    client.SendAsync(User.GetAccessToken(), new GetNotificationsByUser()).Result;

                return PartialView(response);
            }
        }
    }
}