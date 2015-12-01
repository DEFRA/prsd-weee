namespace EA.Weee.Web.RazorHelpers
{
    using System;
    using System.Linq.Expressions;
    using System.Web.Mvc;

    public partial class WeeeGds<TModel>
    {
        public MvcHtmlString ScreenReaderLabelFor<TValue>(Expression<Func<TModel, TValue>> expression)
        {
           return gdsHelper.LabelFor(expression, new { @class = "hidden-for-screen-reader" });
        }
    }
}