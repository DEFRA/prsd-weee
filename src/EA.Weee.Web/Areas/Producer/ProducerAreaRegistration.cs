namespace EA.Weee.Web.Areas.Producer
{
    using Controllers;
    using Infrastructure;
    using System.Web.Mvc;
    using EA.Weee.Core.Helpers;

    public class ProducerAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Producer";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapLowercaseDashedRoute(
                name: "Producer_default",
                url: "Producer/{organisationId}/{directRegistrantId}/{controller}/{action}",
                defaults: new { action = nameof(ProducerController.Index), controller = typeof(ProducerController).GetControllerName(), area = "Producer" },
                namespaces: new[] { typeof(ProducerController).Namespace });

            //context.MapLowercaseDashedRoute(
            //    name: "Producer_submission",
            //    url: "Producer/{organisationId}/{controller}/{action}/{producerSubmissionId}",
            //    defaults: new { action = nameof(ProducerSubmissionController.Index), controller = typeof(ProducerSubmissionController).GetControllerName(), area = "Producer" },
            //    namespaces: new[] { typeof(ProducerSubmissionController).Namespace });
        }
    }
}