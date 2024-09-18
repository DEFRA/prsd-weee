namespace EA.Weee.Web.RazorHelpers
{
    using System.Web.Mvc;

    public partial class WeeeGds<TModel>
    {
        public MvcHtmlString GovUkButtonLink(string linkText, string url, bool secondaryButton = false, string id = null)
        {
            var tag = new TagBuilder("a");
            tag.MergeAttribute("href", url);
            tag.MergeAttribute("role", "button");
            tag.MergeAttribute("draggable", "false");
            tag.MergeAttribute("data-module", "govuk-button");

            if (id != null)
            {
                tag.MergeAttribute("id", id);
            }
          
            tag.AddCssClass("govuk-button");
            if (secondaryButton)
            {
                tag.AddCssClass("govuk-button--secondary");
            }
            tag.SetInnerText(linkText);
            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }
    }
}