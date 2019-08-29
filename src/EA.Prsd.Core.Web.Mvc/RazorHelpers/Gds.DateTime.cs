namespace EA.Prsd.Core.Web.Mvc.RazorHelpers
{
    using System;
    using System.Linq.Expressions;
    using System.Web.Mvc;

    public partial class Gds<TModel>
    {
        private const string longFormat = "d MMMM yyyy";
        private const string longFormatNoYear = "d MMMM";
        private const string shortFormat = "d MMM yyyy";
        private const string shortFormatNoYear = "d MMM";
        private const string rangeFormat = "{0} to {1}";

        public MvcHtmlString DisplayDateFor(Expression<Func<TModel, DateTime>> expression)
        {
            Guard.ArgumentNotNull(() => expression, expression);
            return DisplayDateFor(expression, longFormat);
        }

        public MvcHtmlString DisplayShortDateFor(Expression<Func<TModel, DateTime>> expression)
        {
            Guard.ArgumentNotNull(() => expression, expression);
            return DisplayDateFor(expression, shortFormat);
        }

        public MvcHtmlString DisplayDateRangeFor(Expression<Func<TModel, DateTime>> fromDateExpression, Expression<Func<TModel, DateTime>> toDateExpression)
        {
            Guard.ArgumentNotNull(() => fromDateExpression, fromDateExpression);
            Guard.ArgumentNotNull(() => toDateExpression, toDateExpression);
            return DisplayDateRangeFor(fromDateExpression, toDateExpression, longFormat, longFormatNoYear);
        }

        public MvcHtmlString DisplayShortDateRangeFor(Expression<Func<TModel, DateTime>> fromDateExpression, Expression<Func<TModel, DateTime>> toDateExpression)
        {
            Guard.ArgumentNotNull(() => fromDateExpression, fromDateExpression);
            Guard.ArgumentNotNull(() => toDateExpression, toDateExpression);
            return DisplayDateRangeFor(fromDateExpression, toDateExpression, shortFormat, shortFormatNoYear);
        }

        public MvcHtmlString DisplayDate(DateTime date)
        {
            return DisplayDate(date, longFormat);
        }

        public MvcHtmlString DisplayShortDate(DateTime date)
        {
            return DisplayDate(date, shortFormat);
        }

        public MvcHtmlString DisplayDateRange(DateTime fromDate, DateTime toDate)
        {
            return DisplayDateRange(fromDate, toDate, longFormat, longFormatNoYear);
        }

        public MvcHtmlString DisplayShortDateRange(DateTime fromDate, DateTime toDate)
        {
            return DisplayDateRange(fromDate, toDate, shortFormat, shortFormatNoYear);
        }

        private MvcHtmlString DisplayDateRangeFor(Expression<Func<TModel, DateTime>> fromDateExpression, 
            Expression<Func<TModel, DateTime>> toDateExpression,
            string dateFormat,
            string dateFormatNoYear)
        {
            var compiledFromDateExpression = fromDateExpression.Compile();
            var compiledToDateExpression = toDateExpression.Compile();
            var fromDate = compiledFromDateExpression(htmlHelper.ViewData.Model);
            var toDate = compiledToDateExpression(htmlHelper.ViewData.Model);

            return DisplayDateRange(fromDate, toDate, dateFormat, dateFormatNoYear);
        }

        private static MvcHtmlString DisplayDateRange(DateTime fromDate, DateTime toDate, string dateFormat,
            string dateFormatNoYear)
        {
            return new MvcHtmlString(
                string.Format(rangeFormat,
                    fromDate.ToString(fromDate.Year == toDate.Year ? dateFormatNoYear : dateFormat),
                    toDate.ToString(dateFormat)));
        }

        private MvcHtmlString DisplayDateFor(Expression<Func<TModel, DateTime>> expression, string format)
        {
            var compiledExpression = expression.Compile();
            var date = compiledExpression(htmlHelper.ViewData.Model);
            return DisplayDate(date, format);
        }

        private MvcHtmlString DisplayDate(DateTime date, string format)
        {
            return new MvcHtmlString(date.ToString(format));
        }
    }
}
