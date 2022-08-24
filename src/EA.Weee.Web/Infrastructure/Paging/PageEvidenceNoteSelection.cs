namespace EA.Weee.Web.Infrastructure.Paging
{
    using System;
    using System.Text;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;

    public class PageEvidenceNoteSelection : Pager
    {
        public PageEvidenceNoteSelection(HtmlHelper htmlHelper, int pageSize, int currentPage, int totalItemCount) : base(htmlHelper, pageSize, currentPage, totalItemCount)
        {
        }

        public override string ToHtmlString()
        {
            var model = BuildPaginationModel(GeneratePageUrl);

            if (!string.IsNullOrEmpty(this.PagerOptions.DisplayTemplate))
            {
                var templatePath = $"DisplayTemplates/{this.PagerOptions.DisplayTemplate}";
                return HtmlHelper.Partial(templatePath, model).ToHtmlString();
            }
            else
            {
                var sb = new StringBuilder();

                foreach (var paginationLink in model.PaginationLinks)
                {
                    if (paginationLink.Active)
                    {
                        if (paginationLink.IsCurrent)
                        {
                            sb.AppendFormat("<span class=\"current\">{0}</span>", paginationLink.DisplayText);
                        }
                        else if (!paginationLink.PageIndex.HasValue)
                        {
                            sb.AppendFormat(paginationLink.DisplayText);
                        }
                        else
                        {
                            var link = $@"<button class=""govuk-back-link"" type=""submit"" name=""PageNumber"" value=""{paginationLink.PageIndex}"" form=""transfer-summary-partial-form"">{paginationLink.DisplayText}</button>";
                            
                            sb.Append(link);
                        }
                    }
                    else
                    {
                        if (!paginationLink.IsSpacer)
                        {
                            sb.AppendFormat("<span class=\"disabled\">{0}</span>", paginationLink.DisplayText);
                        }
                        else
                        {
                            sb.AppendFormat("<span class=\"spacer\">{0}</span>", paginationLink.DisplayText);
                        }
                    }
                }
                return sb.ToString();
            }
        }
    }
}