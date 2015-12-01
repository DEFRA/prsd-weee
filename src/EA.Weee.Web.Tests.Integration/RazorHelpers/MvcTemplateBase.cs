namespace EA.Weee.Web.Tests.Integration.RazorHelpers
{
    using System.Web.Mvc;
    using RazorEngine.Templating;

    public class MvcTemplateBase<T> : HtmlTemplateBase<T>
    {
        public HtmlHelper<T> HtmlHelper { get; private set; }

        public MvcTemplateBase()
        {
            var customWebViewPage = new CustomWebViewPage<T>();

            HtmlHelper = customWebViewPage.Html;
        }

        public new string ResolveUrl(string path)
        {
            // Do nothing
            return string.Empty;
        }
    }
}
