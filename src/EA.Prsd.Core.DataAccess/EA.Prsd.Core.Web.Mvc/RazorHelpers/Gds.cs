namespace EA.Prsd.Core.Web.Mvc.RazorHelpers
{
    using System.Collections.Generic;
    using System.Web.Mvc;

    public partial class Gds<TModel>
    {
        protected readonly HtmlHelper<TModel> HtmlHelper;

        public Gds(HtmlHelper<TModel> htmlHelper)
        {
            HtmlHelper = htmlHelper;
        }

        protected static void AddFormControlCssClass(IDictionary<string, object> htmlAttributes)
        {
            if (htmlAttributes.ContainsKey("class"))
            {
                if (!htmlAttributes["class"].ToString().Contains("form-control"))
                {
                    htmlAttributes["class"] += " form-control";
                }
            }
            else
            {
                htmlAttributes.Add("class", "form-control");
            }
        }
    }
}