namespace EA.Weee.Web.Areas.Test.Controllers
{
    using EA.Weee.Api.Client;
    using EA.Weee.Api.Client.Models.Pay;
    using EA.Weee.Web.Areas.Test.ViewModels.ApiIntegration;
    using EA.Weee.Web.Services;
    using Newtonsoft.Json;
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class ApiIntegrationController : TestControllerBase
    {
        private readonly Func<IAddressLookupClient> addressLookupClient;
        private readonly Func<ICompaniesHouseClient> companiesHouseClient;
        private readonly Func<IPayClient> payClient;
        private readonly ISecureReturnUrlHelper secureReturnUrlHelper;
        private readonly IAppConfiguration appConfiguration;

        public ApiIntegrationController(Func<IAddressLookupClient> addressLookupClient, IAppConfiguration appConfiguration, Func<ICompaniesHouseClient> companiesHouseClient, Func<IPayClient> payClient, ISecureReturnUrlHelper secureReturnUrlHelper)
        {
            this.addressLookupClient = addressLookupClient;
            this.appConfiguration = appConfiguration;
            this.companiesHouseClient = companiesHouseClient;
            this.payClient = payClient;
            this.secureReturnUrlHelper = secureReturnUrlHelper;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View(new ApiModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddressLookup(ApiModel viewModel)
        {
            viewModel.SubmittedForm = "postcode";

            // Only validate PostcodeValue when this form is submitted
            if (string.IsNullOrEmpty(viewModel.PostcodeValue))
            {
                ModelState.AddModelError("PostcodeValue", "Please enter a postcode");
                return View("Index", viewModel);
            }
 
            using (var client = addressLookupClient())
            {
                var result = await client.GetAddressesAsync(appConfiguration.AddressLookupReferencePath, viewModel.PostcodeValue);

                viewModel.Result = JsonConvert.SerializeObject(result, new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore
                });
            }
            
            return View(nameof(Index), viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CompaniesHouseLookup(ApiModel viewModel)
        {
            viewModel.SubmittedForm = "company";

            // Only validate CompanyValue when this form is submitted
            if (string.IsNullOrEmpty(viewModel.CompanyValue))
            {
                ModelState.AddModelError("CompanyValue", "Please enter a company number");
                return View(nameof(Index), viewModel);
            }

            using (var client = companiesHouseClient())
            {
                var result = await client.GetCompanyDetailsAsync(appConfiguration.CompaniesHouseReferencePath, viewModel.CompanyValue);

                viewModel.Result = JsonConvert.SerializeObject(result, new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore
                });
            }

            return View(nameof(Index), viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Payment(ApiModel viewModel)
        {
            viewModel.SubmittedForm = "payment";

            using (var client = payClient())
            {
                var id = Guid.NewGuid();
                var secureId = secureReturnUrlHelper.GenerateSecureRandomString(id);
                var returnUrl = string.Format(appConfiguration.GovUkPayReturnBaseUrl, id, secureId);

                var result = await client.CreatePaymentAsync(Guid.NewGuid().ToString(), new CreateCardPaymentRequest()
                {
                    Amount = 1,
                    Description = $"WEEE TEST TRANSACTION {DateTime.UtcNow.Ticks.ToString()}",
                    Email = "testtransaction@email.com",
                    Reference = $"testtransactionref{DateTime.UtcNow.Ticks.ToString()}",
                    ReturnUrl = returnUrl
                });

                viewModel.Result = JsonConvert.SerializeObject(result, new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore
                });
            }

            return View(nameof(Index), viewModel);
        }
    }
}