namespace EA.Weee.Web.Infrastructure.Paging
{
    using System;
    using System.Text;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;

    public class PageEvidenceNoteSelection : Pager
    {
        public string FormName { get; set; }

        public PageEvidenceNoteSelection(HtmlHelper htmlHelper, int pageSize, int currentPage, int totalItemCount, string formName) : base(htmlHelper, pageSize, currentPage, totalItemCount)
        {
            FormName = formName;
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
                            var link = $@"<button class=""link-like-behaviour"" type=""submit"" name=""PageNumber"" value=""{paginationLink.PageIndex}"" form=""{FormName}""><span>{paginationLink.DisplayText}</span></button>";
                            
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