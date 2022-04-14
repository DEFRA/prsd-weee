namespace EA.Prsd.Core.Web.Mvc.RazorHelpers
{
    using System.Collections.Generic;
    using System.Web.Mvc;

    public static class GdsExtensions
    {
        public static Gds<TModel> Gds<TModel>(this HtmlHelper<TModel> htmlHelper)
        {
            return new Gds<TModel>(htmlHelper);
        }

        public static void AddFormControlCssClass(IDictionary<string, object> htmlAttributes, bool useHalfWidth = true)
        {
            if (htmlAttributes.ContainsKey("class"))
            {
                if (!htmlAttributes["class"].ToString().Contains("govuk-!-width-one-half") && useHalfWidth)
                {
                    htmlAttributes["class"] += " govuk-!-width-one-half";
                }
                if (!htmlAttributes["class"].ToString().Contains("form-control"))
                {
                    htmlAttributes["class"] += " form-control";
                }
            }
            else
            {
                if (useHalfWidth)
                {
                    htmlAttributes.Add("class", "govuk-!-width-one-half form-control");
                }
                else
                {
                    htmlAttributes.Add("class", "form-control");
                }
            }
        }

        public static void AddClass(IDictionary<string, object> htmlAttributes, string cssClass)
        {
            if (htmlAttributes.ContainsKey("class"))
            {
                if (!htmlAttributes["class"].ToString().Contains(cssClass))
                {
                    htmlAttributes["class"] += string.Format(" {0}", cssClass);
                }
            }
            else
            {
                htmlAttributes.Add("class", cssClass);
            }
        }
    }
}