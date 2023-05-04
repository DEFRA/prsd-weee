namespace EA.Weee.Web.RazorHelpers
{
    using Prsd.Core;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public partial class WeeeGds<TModel>
    {
        public MvcHtmlString TableSummary(string caption, Dictionary<string, object> data, string columnHeading = null, string columnDescription = null)
        {
            Guard.ArgumentNotNullOrEmpty(() => caption, caption);

            var html = "<div class=\"govuk-form-group\"><table class=\"govuk-table\">" +
                       $"<caption class=\"govuk-table__caption\"><span class=\"govuk-visually-hidden\">{caption}</span></caption>";

            html += "<thead class=\"govuk-table__head\">";

            if (columnDescription != null && columnHeading != null)
            {
                html += $"<tr class=\"govuk-table__row govuk-visually-hidden\"><th class=\"govuk-table__header govuk-visually-hidden\" scope=\"col\">{columnHeading}</th>" +
                        $"<th class=\"govuk-table__header govuk-visually-hidden\" scope=\"col\">{columnDescription}</th></tr>";
            }
            else
            {
                html += "<tr class=\"govuk-table__row govuk-visually-hidden\"><th class=\"govuk-table__header\" scope=\"col\" colspan=\"2\"></th></tr>";
            }

            html += "</thead><tbody class=\"govuk-table__body\">";

            foreach (var key in data.Keys)
            {
                html += $"<tr class=\"govuk-table__row\"><th scope=\"row\" class=\"govuk-table__header\">{key}</th>";
                html += $"<td class=\"govuk-table__cell\">{data[key]}</td></tr>";
            }

            html += "</tbody></table></div>";

            return new MvcHtmlString(html);
        }
    }
}