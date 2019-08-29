namespace EA.Prsd.Core.Web.Mvc.RazorHelpers
{
    using System.Web.Mvc;

    public partial class Gds<TModel>
    {
        public MvcHtmlString HintSpan(string hintText, string id = null)
        {
            var hint = CreateHintTag(hintText, id, "span");
            return MvcHtmlString.Create(hint.ToString());
        }

        public MvcHtmlString HintParagraph(string hintText, string id = null)
        {
            var hint = CreateHintTag(hintText, id, "p");
            return MvcHtmlString.Create(hint.ToString());
        }

        private static TagBuilder CreateHintTag(string hintText, string id, string tagName)
        {
            var hint = new TagBuilder(tagName);
            hint.AddCssClass("form-hint");
            hint.SetInnerText(hintText);
            if (!string.IsNullOrWhiteSpace(id))
            {
                hint.GenerateId(id);
            }
            return hint;
        }
    }
}