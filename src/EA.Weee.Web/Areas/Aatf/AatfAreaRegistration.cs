namespace EA.Weee.Web.Areas.Aatf
{
    using Aatf.Controllers;
    using Infrastructure;
    using System.Web.Mvc;
    using Core.AatfEvidence;

    public class AatfAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Aatf";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.Routes.MapMvcAttributeRoutes();

            context.MapLowercaseDashedRoute(
                name: AatfEvidenceRedirect.ViewSubmittedEvidenceRouteName,
                url: "Aatf/{organisationId}/manage-evidence-notes/{aatfId}/view-submitted-evidence-note/{evidenceNoteId}",
                defaults: new { action = "ViewDraftEvidenceNote", controller = "ManageEvidenceNotes", noteStatus = NoteStatus.Submitted },
                namespaces: new[] { typeof(ManageEvidenceNotesController).Namespace });

            context.MapLowercaseDashedRoute(
                name: AatfEvidenceRedirect.ViewEvidenceRouteName,
                url: "Aatf/{organisationId}/manage-evidence-notes/{aatfId}/view-draft-evidence-note/{evidenceNoteId}",
                defaults: new { action = "ViewDraftEvidenceNote", controller = "ManageEvidenceNotes", noteStatus = NoteStatus.Draft },
                namespaces: new[] { typeof(ManageEvidenceNotesController).Namespace });

            context.MapLowercaseDashedRoute(
                name: AatfEvidenceRedirect.ManageEvidenceRouteName,
                url: "Aatf/{organisationId}/{controller}/{aatfId}/{action}",
                defaults: new { action = "Index", controller = "ManageEvidenceNotes" },
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