namespace EA.Weee.Web.RazorHelpers
{
    using System.Web.Mvc;

    public static class WeeeGdsExtensions
    {
        public static WeeeGds<TModel> WeeeGds<TModel>(this WebViewPage<TModel> webViewPage)
        {
            return new WeeeGds<TModel>(webViewPage);
        }
    }
}