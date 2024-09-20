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
            }
            else
            {
                html += $"<caption class=\"govuk-table__caption govuk-visually-hidden\">{caption}</caption>";
            }

            // Adding table headers with govuk-visually-hidden class for accessibility
            html += "<thead class=\"govuk-table__head\">";

            // Always add hidden headers for screen readers even if columnHeading and columnDescription are null
            html += "<tr class=\"govuk-table__row\">";

            // Hidden header for key/row label column
            if (!string.IsNullOrEmpty(columnHeading))
            {
                html += $"<th scope=\"col\" class=\"govuk-table__header govuk-visually-hidden\">{columnHeading}</th>";
            }
            else
            {
                html += "<th scope=\"col\" class=\"govuk-table__header govuk-visually-hidden\">Item</th>";
            }

            // Hidden header for value column
            if (!string.IsNullOrEmpty(columnDescription))
            {
                html += $"<th scope=\"col\" class=\"govuk-table__header govuk-visually-hidden\">{columnDescription}</th>";
            }
            else
            {
                html += "<th scope=\"col\" class=\"govuk-table__header govuk-visually-hidden\">Details</th>";
            }

            html += "</tr>";
            html += "</thead>";

            // Add table body
            html += "<tbody class=\"govuk-table__body\">";

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
