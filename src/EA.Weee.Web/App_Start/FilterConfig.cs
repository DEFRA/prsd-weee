namespace EA.Weee.Web
{
    using System.Web.Mvc;
    using Infrastructure;
    using Prsd.Core.Web.Mvc.Filters;

    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new RequireHttpsAttribute());
            filters.Add(new HandleApiErrorAttribute());
            filters.Add(new HandleErrorAttribute() { View = "~/Views/Errors/InternalError.cshtml" });
            filters.Add(new AuthorizeAttribute());
            filters.Add(new UserAccountActivationAttribute());
            filters.Add(new AntiForgeryErrorFilter());
        }
    }
}