namespace EA.Weee.Web.Areas.AatfReturn
{
    using System.Web.Mvc;
    using AATF_Return.Controllers;
    using Infrastructure;

    public class AatfReturnAreaRegistration : AreaRegistration 
    {
        public override string AreaName => "AatfReturn";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapLowercaseDashedRoute(
                name: "AatfReturn_default",
                url: "AatfReturn/{organisationId}/{controller}/{action}/{entityId}",
                defaults: new { action = "Index", controller = "NonObligated", entityId = UrlParameter.Optional },
                namespaces: new[] { typeof(NonObligatedController).Namespace });
        }
    }
}