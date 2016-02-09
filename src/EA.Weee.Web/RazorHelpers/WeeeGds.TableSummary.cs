namespace EA.Weee.Web.RazorHelpers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Prsd.Core;

    public partial class WeeeGds<TModel>
    {
        public MvcHtmlString TableSummary(string caption, Dictionary<string, object> data)
        {
            Guard.ArgumentNotNullOrEmpty(() => caption, caption);

            var html = string.Format("<div class=\"form-group\"><table><caption><span class=\"hidden-for-screen-reader\">{0}</span></caption><thead><tr><th colspan=\"2\"></th></tr></thead><tbody>", caption);

            foreach (var key in data.Keys)
            {
                html += string.Format("<tr><td class=\"grey-text\" style=\"width: 250px\">{0}</td>", key);
                html += string.Format("<td class=\"black-text\">{0}</td></tr>", data[key]);
            }

            html += "</tbody></table></div>";

            return new MvcHtmlString(html);
        }
    }
}