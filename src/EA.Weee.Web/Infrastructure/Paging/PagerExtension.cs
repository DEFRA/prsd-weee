namespace EA.Weee.Web.Infrastructure.Paging
{
    using System.Web.Mvc;
 
    public static class PagerExtensions
    {
        public static Pager Pager(this HtmlHelper htmlHelper, int pageSize, int currentPage, int totalItemCount)
        {
            return new Pager(htmlHelper, pageSize, currentPage, totalItemCount);
        }

        public static Pager<TModel> Pager<TModel>(this HtmlHelper<TModel> htmlHelper, int pageSize, int currentPage,
            int totalItemCount)
        {
            return new Pager<TModel>(htmlHelper, pageSize, currentPage, totalItemCount);
        }
    }
}