namespace EA.Weee.Web.Areas.AeReturn
{
    using System.Web.Mvc;

    public class AeReturnAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "AeReturn";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "AeReturn_default",
                "AeReturn/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional });
        }
    }
}