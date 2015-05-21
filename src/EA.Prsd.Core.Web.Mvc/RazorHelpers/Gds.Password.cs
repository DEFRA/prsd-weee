namespace EA.Prsd.Core.Web.Mvc.RazorHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using System.Web.Routing;

    public partial class Gds<TModel>
    {
        public MvcHtmlString PasswordFor<TValue>(Expression<Func<TModel, TValue>> expression)
        {
            return PasswordFor(expression, new RouteValueDictionary());
        }

        public MvcHtmlString PasswordFor<TValue>(Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            var routeValueDictionary = System.Web.Mvc.HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            return PasswordFor(expression, routeValueDictionary);
        }

        public MvcHtmlString PasswordFor<TValue>(Expression<Func<TModel, TValue>> expression,
            IDictionary<string, object> htmlAttributes)
        {
            AddFormControlCssClass(htmlAttributes);
            return HtmlHelper.PasswordFor(expression, htmlAttributes);
        }
    }
}