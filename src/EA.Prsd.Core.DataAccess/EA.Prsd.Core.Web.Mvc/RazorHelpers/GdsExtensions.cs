namespace EA.Prsd.Core.Web.Mvc.RazorHelpers
{
    using System.Web.Mvc;

    public static class GdsExtensions
    {
        public static Gds<TModel> Gds<TModel>(this HtmlHelper<TModel> htmlHelper)
        {
            return new Gds<TModel>(htmlHelper);
        }
    }
}