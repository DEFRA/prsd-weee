namespace EA.Weee.Web.Controllers
{
    using EA.Weee.Web.ViewModels.Payment;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Web.Mvc;

    public class PaymentController : Controller
    {
        private string baseUrl = "https://publicapi.payments.service.gov.uk/v1/";
        private string apiKey = "Bearer api_test_ko7qtjqqa2l8jgmhmin77p1s4qjgmebv2futglsv58b4u9o4blt1b4vnkb";

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
            IEnumerable<PaymentRequest> reqs = null;
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
                    var readTask = result.Content.ReadAsAsync<IList<PaymentRequest>>();
                    readTask.Wait();

                    reqs = readTask.Result;
                }
                else
                {
                    //log response status here..

                    reqs = Enumerable.Empty<PaymentRequest>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View(reqs);

            return View("Payment");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult CreatePayment()
        {
            return Content("<script>window.location = 'https://www.gov.uk/payments/test-weee-service/pay-for-eee-producer-registration';</script>");
        }
    }
}