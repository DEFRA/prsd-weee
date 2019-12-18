namespace EA.Weee.Web
{
    using EA.Weee.Web.Services;
    using Filters;
    using Infrastructure;
    using Prsd.Core.Web.Mvc.Filters;
    using System.Web.Mvc;
    
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters, IAppConfiguration configuration)
        {
            if (configuration.MaintenanceMode)
            {
                filters.Add(new MaintenanceModeFilterAttribute());
            }
            
            filters.Add(new RequireHttpsAttribute());
            filters.Add(new HandleApiErrorAttribute());
            filters.Add(new HandleErrorAttribute() { View = "~/Views/Errors/InternalError.cshtml" });
            filters.Add(new RenderActionErrorAttribute());
            filters.Add(new AuthorizeAttribute());
            filters.Add(new UserAccountActivationAttribute());
            filters.Add(new AntiForgeryErrorFilter());
        }
    }
}