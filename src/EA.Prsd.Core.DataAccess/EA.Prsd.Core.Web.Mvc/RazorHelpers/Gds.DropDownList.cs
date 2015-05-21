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
        public MvcHtmlString DropDownListFor<TValue>(Expression<Func<TModel, TValue>> expression,
            IEnumerable<SelectListItem> selectList)
        {
            return DropDownListFor(expression, selectList, new RouteValueDictionary());
        }

        public MvcHtmlString DropDownListFor<TValue>(Expression<Func<TModel, TValue>> expression,
            IEnumerable<SelectListItem> selectList, object htmlAttributes)
        {
            var routeValueDictionary = System.Web.Mvc.HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            return DropDownListFor(expression, selectList, routeValueDictionary);
        }

        public MvcHtmlString DropDownListFor<TValue>(Expression<Func<TModel, TValue>> expression,
            IEnumerable<SelectListItem> selectList,
            IDictionary<string, object> htmlAttributes)
        {
            AddFormControlCssClass(htmlAttributes);
            return HtmlHelper.DropDownListFor(expression, selectList, htmlAttributes);
        }
    }
}