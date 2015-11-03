namespace EA.Weee.Web.RazorHelpers
{
    using System.Web.Mvc;
    using Prsd.Core.Web.Mvc.RazorHelpers;

    public partial class WeeeGds<TModel>
    {
        public readonly HtmlHelper<TModel> HtmlHelper;
        public readonly UrlHelper UrlHelper;

        private readonly Gds<TModel> gdsHelper;

        public WeeeGds(HtmlHelper<TModel> htmlHelper)
        {
            HtmlHelper = htmlHelper;
            gdsHelper = new Gds<TModel>(htmlHelper);
        }

        public WeeeGds(WebViewPage<TModel> webViewPage)
            : this(webViewPage.Html)
        {
            UrlHelper = webViewPage.Url;
        }

        public ProgressiveDisclosure<TModel> ProgressiveDisclosure(string linkText)
        {
            return new ProgressiveDisclosure<TModel>(this, linkText);
        }
    }
}