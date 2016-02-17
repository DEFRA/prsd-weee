namespace EA.Weee.Web.RazorHelpers
{
    using System.Web.Mvc;
    public partial class WeeeGds<TModel>
    {
        public MvcHtmlString Tab(string displayText, string url, bool isActive)
        {
            var linkBuilder = new TagBuilder("a");
            linkBuilder.Attributes.Add("href", url);
            linkBuilder.SetInnerText(displayText);
            var tagBuilder = new TagBuilder("li");
            tagBuilder.InnerHtml = linkBuilder.ToString();
            if (isActive)
            {
                tagBuilder.Attributes.Add("class", "active");
            }
            return new MvcHtmlString(tagBuilder.ToString());
        }
    }
}