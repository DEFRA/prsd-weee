using System.Web;
using System.Web.Mvc;

namespace EA.Weee.Web.Maintenance
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
