namespace EA.Weee.Web.RazorHelpers
{
    using Prsd.Core;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public partial class WeeeGds<TModel>
    {
        public MvcHtmlString TableSummary(string caption, Dictionary<string, object> data, string columnHeading = null, string columnDescription = null, bool displayCaption = false)
        {
            Guard.ArgumentNotNullOrEmpty(() => caption, caption);

            var html = "<div class=\"govuk-form-group\"><table class=\"govuk-table\">";

            if (displayCaption)
            {
                html += $"<caption class=\"govuk-table__caption\" style=\"font-size: 24px;\">{caption}</caption>";
                // Add column widths only when displayCaption is true
                html += "<thead class=\"govuk-table__head\"><tr class=\"govuk-table__row\"><th class=\"govuk-table__header\" scope=\"col\" style=\"width: 50%;\"></th>" +
                        "<th class=\"govuk-table__header\" scope=\"col\" style=\"width: 50%;\"></th></tr></thead>";
            }
            else
            {
                html += $"<caption class=\"govuk-table__caption govuk-visually-hidden\">{caption}</caption>";
                html += "<thead class=\"govuk-table__head\"><tr class=\"govuk-table__row\"><th class=\"govuk-table__header\" scope=\"col\"></th>" +
                        "<th class=\"govuk-table__header\" scope=\"col\"></th></tr></thead>";
            }

            html += "<tbody class=\"govuk-table__body\">";

            foreach (var key in data.Keys)
            {
                if (displayCaption)
                {
                    html += $"<tr class=\"govuk-table__row\"><th scope=\"row\" class=\"govuk-table__header\" style=\"width: 50%;\">{key}</th>";
                    html += $"<td class=\"govuk-table__cell\" style=\"width: 50%;\">{data[key]}</td></tr>";
                }
                else
                {
                    html += $"<tr class=\"govuk-table__row\"><th scope=\"row\" class=\"govuk-table__header\">{key}</th>";
                    html += $"<td class=\"govuk-table__cell\">{data[key]}</td></tr>";
                }
            }

            html += "</tbody></table></div>";

            return new MvcHtmlString(html);
        }
    }
}
