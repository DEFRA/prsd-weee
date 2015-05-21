namespace EA.Weee.Web
{
    using System.Web.Mvc;
    using Infrastructure;
    using Prsd.Core.Web.Mvc.Filters;

    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleApiErrorAttribute());
            filters.Add(new OrganisationRequiredAttribute());
        }
    }
}