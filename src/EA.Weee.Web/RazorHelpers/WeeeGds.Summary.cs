namespace EA.Weee.Web.RazorHelpers
{
    public partial class WeeeGds<TModel>
    {
        public void SummaryExpansion(string summaryText, string htmlclass, string eventCategory, string eventAction, string eventLabel = null)
        {
            var html = string.Format(
                @"<summary class = ""{0}"" onclick=""if(this.getAttribute('aria-expanded') == 'true'){{{1}}}"">{2}</summary>", htmlclass, EventTrackingFunction(eventCategory, eventAction, eventLabel), summaryText);

            HtmlHelper.ViewContext.Writer.Write(html);
        }
    }
}