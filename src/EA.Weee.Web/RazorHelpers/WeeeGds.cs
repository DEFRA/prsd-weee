namespace EA.Weee.Web.RazorHelpers
{
    using System.Web.Mvc;
    using Prsd.Core.Web.Mvc.RazorHelpers;

    public partial class WeeeGds<TModel>
    {
        protected readonly HtmlHelper<TModel> HtmlHelper;

        private readonly Gds<TModel> gdsHelper;

        public WeeeGds(HtmlHelper<TModel> htmlHelper)
        {
            HtmlHelper = htmlHelper;
            gdsHelper = new Gds<TModel>(htmlHelper);
        }
    }
}