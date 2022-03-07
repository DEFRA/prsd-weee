namespace EA.Prsd.Core.Web.Mvc.Tests.Helpers
{
    using System;
    using System.Linq.Expressions;
    using System.Web.Mvc;
    using RazorHelpers;

    public class GdsExtensionTester<TModel> : Gds<TModel>
    {
        public GdsExtensionTester(HtmlHelper<TModel> htmlHelper) : base(htmlHelper)
        {
        }

        public static new string GetPropertyName<TProperty>(HtmlHelper htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return Gds<TModel>.GetPropertyName(htmlHelper, expression);
        }
    }
}
