namespace EA.Weee.Web.Infrastructure.Paging
{
    using System;
    using System.Web.Mvc;

    public class Pager<TModel> : Pager
    {
        private HtmlHelper<TModel> htmlHelper;

        public Pager(HtmlHelper<TModel> htmlHelper, int pageSize, int currentPage, int totalItemCount)
            : base(htmlHelper, pageSize, currentPage, totalItemCount)
        {
            this.htmlHelper = htmlHelper;
        }

        public Pager<TModel> Options(Action<PagerOptionsBuilder<TModel>> buildOptions)
        {
            buildOptions(new PagerOptionsBuilder<TModel>(this.PagerOptions, htmlHelper));
            return this;
        }
    }
}