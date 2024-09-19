namespace EA.Weee.Web.Areas.Payment.Controllers
{
    using EA.Weee.Core.Helpers;
    using EA.Weee.Web.Areas.Producer.Controllers;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class PaymentController : ExternalSiteController
    {
        private readonly IPaymentService paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            this.paymentService = paymentService;
        }

        [HttpGet]
        public async Task<ActionResult> PaymentResult(string token)
        {
            var success = await paymentService.HandlePaymentReturnAsync(User.GetAccessToken(), token);

            if (!string.IsNullOrWhiteSpace(success.PaymentReference))
            {
                return RedirectToAction(nameof(ProducerSubmissionController.PaymentSuccess), typeof(ProducerSubmissionController).GetControllerName(), 
                    new { directRegistrantId = success.DirectRegistrantId, area = "Producer" });
            }

            return RedirectToAction(nameof(ProducerSubmissionController.PaymentFailure), typeof(ProducerSubmissionController).GetControllerName(), 
                new { directRegistrantId = success.DirectRegistrantId, area = "Producer"});
        }
    }
}