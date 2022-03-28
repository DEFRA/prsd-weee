namespace EA.Weee.Web.Areas.Aatf
{
    using Aatf.Controllers;
    using Infrastructure;
    using System.Web.Mvc;

    public class AatfAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Aatf";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.Routes.MapMvcAttributeRoutes();

            context.MapLowercaseDashedRoute(
                name: AatfEvidenceRedirect.ViewEvidenceRouteName,
                url: "Aatf/{organisationId}/{controller}/{action}/{evidenceNoteId}",
                defaults: new { action = "ViewDraftEvidenceNote", controller = "ManageEvidenceNotes" },
                namespaces: new[] { typeof(ManageEvidenceNotesController).Namespace });

            context.MapLowercaseDashedRoute(
                name: AatfEvidenceRedirect.ManageEvidenceRouteName,
                url: "Aatf/{organisationId}/manage-evidence/{aatfId}/{action}/",
                defaults: new { action = "Index", controller = "ManageEvidenceNotes", aatfId = UrlParameter.Optional },
                namespaces: new[] { typeof(ManageEvidenceNotesController).Namespace });

            context.MapLowercaseDashedRoute(
                name: "Aatf_default",
                url: "Aatf/{organisationId}/{controller}/{action}/",
                defaults: new { action = "Index", controller = "Home" },
                namespaces: new[] { typeof(HomeController).Namespace });

            context.MapLowercaseDashedRoute(
                name: "Aatf_ContactDetails",
                url: "Aatf/{organisationId}/{controller}/{action}/{aatfId}",
                defaults: new { action = "Index", controller = "ViewAatfContactDetails" },
                namespaces: new[] { typeof(ContactDetailsController).Namespace });
        }
    }
}