namespace EA.Weee.Web.Areas.Payment
{
    using Controllers;
    using Infrastructure;
    using System.Web.Mvc;
    using EA.Weee.Core.Helpers;

    public class PaymentAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Payment";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapLowercaseDashedRoute(
                name: "payment_default",
                url: "Payment/{controller}/{action}",
                defaults: new { action = nameof(PaymentController.PaymentResult), controller = typeof(PaymentController).GetControllerName(), area = "Payment" },
                namespaces: new[] { typeof(PaymentController).Namespace });
        }
    }
}