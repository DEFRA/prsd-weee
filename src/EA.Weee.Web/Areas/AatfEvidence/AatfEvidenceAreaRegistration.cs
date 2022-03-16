namespace EA.Weee.Web.Areas.AatfEvidence
{
    using System.Web.Mvc;
    using Controllers;
    using Infrastructure;
    using HoldingController = Controllers.HoldingController;

    public class AatfEvidenceAreaRegistration : AreaRegistration 
    {
        public override string AreaName => "AatfEvidence";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.Routes.MapMvcAttributeRoutes();

            context.MapLowercaseDashedRoute(
                name: AatfEvidenceRedirect.Holding,
                url: "aatf-evidence/holding/{organisationId}",
                defaults: new { action = "Index", controller = "Holding" },
                namespaces: new[] { typeof(HoldingController).Namespace });

            context.MapLowercaseDashedRoute(
                name: AatfEvidenceRedirect.Create,
                url: "aatf-evidence/{organisationId}/manage-evidence-notes/create-evidence-note",
                defaults: new { action = "Create", controller = "Note" },
                namespaces: new[] { typeof(NoteController).Namespace });

            context.MapLowercaseDashedRoute(
                name: AatfEvidenceRedirect.Default,
                url: "aatf-evidence/{organisationId}/choose-site/",
                defaults: new { action = "Index", controller = "SelectYourAatf" },
                namespaces: new[] { typeof(SelectYourAatfController).Namespace });
        }
    }
}