namespace EA.Weee.Web.Areas.Scheme
{
    using Controllers;
    using Infrastructure;
    using System.Web.Mvc;

    public class SchemeAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Scheme";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapLowercaseDashedRoute(
                name: "Scheme_view_Transfer",
                url: "Scheme/{pcsId}/transfer-evidence/outgoing-transfers/draft-transfer/{evidenceNoteId}",
                defaults: new { action = "TransferredEvidence", controller = "TransferEvidence", area = "Scheme" },
                namespaces: new[] { typeof(ManageEvidenceNotesController).Namespace });

            context.MapLowercaseDashedRoute(
                name: "Scheme_evidence_default",
                url: "Scheme/{pcsId}/{controller}/{action}/{evidenceNoteId}",
                defaults: new { action = "Index", controller = "Home", area = "Scheme" },
                namespaces: new[] { typeof(ManageEvidenceNotesController).Namespace });

            context.MapLowercaseDashedRoute(
                name: "Scheme_default",
                url: "Scheme/{pcsId}/{controller}/{action}/{entityId}",
                defaults: new { action = "Index", controller = "Home", entityId = UrlParameter.Optional, area = "Scheme" },
                namespaces: new[] { typeof(HomeController).Namespace });
        }
    }
}