﻿namespace EA.Weee.Web.Areas.Producer
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
                url: "Producer/{directRegistrantId}/{controller}/{action}",
                defaults: new { action = nameof(ProducerController.TaskList), controller = typeof(ProducerController).GetControllerName(), area = "Producer" },
                namespaces: new[] { typeof(ProducerController).Namespace });

            context.MapLowercaseDashedRoute(
                name: "Producer_submission",
                url: "Producer/{directRegistrantId}/{controller}/{action}",
                defaults: new { action = nameof(ProducerSubmissionController.EditOrganisationDetails), controller = typeof(ProducerSubmissionController).GetControllerName(), area = "Producer" },
                namespaces: new[] { typeof(ProducerSubmissionController).Namespace });
        }
    }
}