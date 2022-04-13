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
                name: "Scheme_default",
                url: "Scheme/{pcsId}/{controller}/{action}/{entityId}",
                defaults: new { action = "Index", controller = "Home", entityId = UrlParameter.Optional, area = "Scheme" },
                namespaces: new[] { typeof(HomeController).Namespace });

            context.MapLowercaseDashedRoute(
                name: "Scheme_manage_evidence",
                url: "Scheme/{organisationId}/{controller}/{action}",
                defaults: new { action = "Index", controller = "ManageEvidenceNotes" },
                namespaces: new[] { typeof(ManageEvidenceNotesController).Namespace });

            context.MapLowercaseDashedRoute(
            name: "Scheme_transfer",
            url: "Scheme/{organisationId}/{controller}/{action}",
            defaults: new { action = "Index", controller = "TrasferEvidenceController" },
            namespaces: new[] { typeof(TrasferEvidenceController).Namespace });
        }
    }
}