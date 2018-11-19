namespace EA.Weee.Web.RazorHelpers
{
    using System;

    public class ProgressiveDisclosure<TModel> : IDisposable
    {
        private readonly WeeeGds<TModel> gdsHelper;

        public ProgressiveDisclosure(WeeeGds<TModel> gdsHelper, string linkText)
        {
            this.gdsHelper = gdsHelper;

            var html = string.Format(
                @"<details class=""govuk-details""><summary class=""govuk-details__summary""><span class=""govuk-details__summary-text"">{1}<span class=""hidden-for-screen-reader"">This is an expandable link that will reveal more content upon activation</span></span></summary><div class=""govuk-details__text"">",
                gdsHelper.EventTrackingFunction("Hidden content", "Progressive disclosure", linkText), linkText);

            gdsHelper.HtmlHelper.ViewContext.Writer.Write(html);
        }

        public void Dispose()
        {
            gdsHelper.HtmlHelper.ViewContext.Writer.Write("</div></details>");
        }
    }
}