namespace EA.Weee.Web.RazorHelpers
{
    using System.Web.Mvc;
    using Prsd.Core.Web.Mvc.RazorHelpers;

    public partial class WeeeGds<TModel>
    {
        public readonly HtmlHelper<TModel> HtmlHelper;
        public readonly UrlHelper UrlHelper;

        private readonly Gds<TModel> gdsHelper;
        public WeeeGds(WebViewPage<TModel> webViewPage)
        {
            HtmlHelper = webViewPage.Html;
            UrlHelper = webViewPage.Url;

            gdsHelper = new Gds<TModel>(webViewPage.Html);
        }

        public ProgressiveDisclosure<TModel> ProgressiveDisclosure(string linkText)
        {
            return new ProgressiveDisclosure<TModel>(this, linkText);
        }
    }
}