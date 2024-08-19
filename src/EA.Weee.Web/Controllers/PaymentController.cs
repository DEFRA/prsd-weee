namespace EA.Weee.Web.Controllers
{
    using EA.Weee.Web.ViewModels.Payment;
    using IdentityModel;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Web.Mvc;

    using EA.Weee.Web.Payment;

    public class PaymentController : Controller
    {
        private string baseUrl = "https://publicapi.payments.service.gov.uk/v1/";
        private string apiKey = "api_test_ko7qtjqqa2l8jgmhmin77p1s4qjgmebv2futglsv58b4u9o4blt1b4vnkb";

        // GET: Payment
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Index()
        {
            return View("Payment");
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult List()
        {
            IEnumerable<PaymentRequestViewModel> reqs = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Add("Authorization", apiKey);
                //HTTP GET
                var responseTask = client.GetAsync("payments");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<PaymentRequestViewModel>>();
                    readTask.Wait();

                    reqs = readTask.Result;
                }
                else
                {
                    //log response status here..

                    reqs = Enumerable.Empty<PaymentRequestViewModel>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View(reqs);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult CreatePayment()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{apiKey}");

                var payClient = new PayClient(client);

                var paymentRequest = new CreateCardPaymentRequest();
                paymentRequest.Amount = 3000;
                paymentRequest.Description = "Pay for EEE Producer Registration";
                paymentRequest.Reference = Guid.NewGuid().ToString();
                paymentRequest.Return_url = "https://localhost:44300/payment/result";

                var paymentRequestTask = payClient.Create_a_paymentAsync(null, paymentRequest);
                paymentRequestTask.Wait();

                var result = paymentRequestTask.Result;
                if (result.State.Status == "created")
                {
                    Session["paymentId"] = result.Payment_id;
                    return Redirect(result._links.Next_url.Href);
                }
            }

            ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");

            return View("Payment");
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Result(string id)
        {
            string paymentId = null;
            if (Session["paymentId"] != null)
            {
                paymentId = Session["paymentId"].ToString();
            }

            if (id != null)
            {
                paymentId = id;
            }

            var paymentResultViewModel = new PaymentResultViewModel();

            if (paymentId != null) 
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{apiKey}");

                    var payClient = new PayClient(client);

                    var getPaymentTask = payClient.Get_a_paymentAsync(paymentId);
                    getPaymentTask.Wait();

                    var paymentWithAllLinks = getPaymentTask.Result;

                    paymentResultViewModel.Amount = paymentWithAllLinks.Amount / 100;
                    paymentResultViewModel.Email = paymentWithAllLinks.Email;
                    paymentResultViewModel.Description = paymentWithAllLinks.Description;
                    paymentResultViewModel.Reference = paymentWithAllLinks.Reference;
                    paymentResultViewModel.Status = paymentWithAllLinks.State.Status;
                    paymentResultViewModel.Created_date = DateTime.Parse(paymentWithAllLinks.Created_date, null, System.Globalization.DateTimeStyles.RoundtripKind);

                    paymentResultViewModel.Card_brand = paymentWithAllLinks.Card_details.Card_brand;
                }
            }
            
            return View("PaymentResult", paymentResultViewModel);
        }
    }
}