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

            var html = string.Format("<div class=\"govuk-form-group\"><table class=\"govuk-table\"><caption class=\"govuk-table__caption\"><span class=\"hidden-for-screen-reader\">{0}</span></caption><thead class=\"govuk-table__head\"><tr class=\"govuk-table__row\"><th class=\"govuk-table__header\" scope=\"col\" colspan=\"2\"></th></tr></thead><tbody class=\"govuk-table__body\">", caption);

            foreach (var key in data.Keys)
            {
                html += string.Format("<tr class=\"govuk-table__row\"><th class=\"govuk-table__header\">{0}</th>", key);
                html += string.Format("<td class=\"govuk-table__cell\">{0}</td></tr>", data[key]);
            }

            html += "</tbody></table></div>";

            return new MvcHtmlString(html);
        }
    }
}