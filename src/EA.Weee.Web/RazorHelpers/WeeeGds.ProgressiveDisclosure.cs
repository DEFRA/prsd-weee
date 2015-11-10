namespace EA.Weee.Web.RazorHelpers
{
    using System;
    using System.Web;

    public class ProgressiveDisclosure<TModel> : IDisposable
    {
        private readonly WeeeGds<TModel> gdsHelper;

        public ProgressiveDisclosure(WeeeGds<TModel> gdsHelper, string linkText)
        {
            this.gdsHelper = gdsHelper;

            var html = string.Format(
                @"<details><summary onclick=""if(this.getAttribute('aria-expanded') == 'true'){{{0}}}""><span class=""summary"">{1}<span class=""hidden-for-screen-reader"">This is an expandable link that will reveal more content upon activation</span></span></summary><div class=""panel-indent"">",
                gdsHelper.EventTrackingFunction("Hidden content", "Progressive disclosure", linkText), linkText);

            gdsHelper.HtmlHelper.ViewContext.Writer.Write(html);
        }

        public void Dispose()
        {
            gdsHelper.HtmlHelper.ViewContext.Writer.Write("</div></details>");
        }
    }
}