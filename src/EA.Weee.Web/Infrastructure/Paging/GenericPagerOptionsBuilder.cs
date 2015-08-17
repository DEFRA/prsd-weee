//The MIT license

//Copyright (c) 2008-2014 Martijn Boland, Bart Lenaerts, Rajeesh CV

//Permission is hereby granted, free of charge, to any person obtaining
//a copy of this software and associated documentation files (the
//"Software"), to deal in the Software without restriction, including
//without limitation the rights to use, copy, modify, merge, publish,
//distribute, sublicense, and/or sell copies of the Software, and to
//permit persons to whom the Software is furnished to do so, subject to
//the following conditions:

//The above copyright notice and this permission notice shall be
//included in all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
namespace EA.Weee.Web.Infrastructure.Paging
{
    using System;
    using System.Linq.Expressions;
    using System.Web.Mvc;
    using System.Web.Routing;

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public class PagerOptionsBuilder<TModel> : PagerOptionsBuilder
    {
        private readonly HtmlHelper<TModel> htmlHelper;

        public PagerOptionsBuilder(PagerOptions pagerOptions, HtmlHelper<TModel> htmlHelper)
            : base(pagerOptions)
        {
            this.htmlHelper = htmlHelper;
        }

        /// <summary>
        /// Adds a strongly typed route value parameter based on the current model.
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="expression"></param>
        /// <example>AddRouteValueFor(m => m.SearchQuery)</example>
        /// <returns></returns>
        public PagerOptionsBuilder<TModel> AddRouteValueFor<TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            var name = ExpressionHelper.GetExpressionText(expression);
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

            AddRouteValue(name, metadata.Model);

            return this;
        }

        /// <summary>
        /// Set the action name for the pager links. Note that we're always using the current controller.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public new PagerOptionsBuilder<TModel> Action(string action)
        {
            base.Action(action);
            return this;
        }

        /// <summary>
        /// Add a custom route value parameter for the pager links.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public new PagerOptionsBuilder<TModel> AddRouteValue(string name, object value)
        {
            base.AddRouteValue(name, value);
            return this;
        }

        /// <summary>
        /// Set custom route value parameters for the pager links.
        /// </summary>
        /// <param name="routeValues"></param>
        /// <returns></returns>
        public new PagerOptionsBuilder<TModel> RouteValues(object routeValues)
        {
            base.RouteValues(routeValues);
            return this;
        }

        /// <summary>
        /// Set custom route value parameters for the pager links.
        /// </summary>
        /// <param name="routeValues"></param>
        /// <returns></returns>
        public new PagerOptionsBuilder<TModel> RouteValues(RouteValueDictionary routeValues)
        {
            base.RouteValues(routeValues);
            return this;
        }

        /// <summary>
        /// Set the name of the DisplayTemplate view to use for rendering.
        /// </summary>
        /// <param name="displayTemplate"></param>
        /// <remarks>The view must have a model of IEnumerable&lt;PaginationModel&gt;</remarks>
        /// <returns></returns>
        public new PagerOptionsBuilder<TModel> DisplayTemplate(string displayTemplate)
        {
            base.DisplayTemplate(displayTemplate);
            return this;
        }

        /// <summary>
        /// Set the maximum number of pages to show. The default is 10.
        /// </summary>
        /// <param name="maxNrOfPages"></param>
        /// <returns></returns>
        public new PagerOptionsBuilder<TModel> MaxNrOfPages(int maxNrOfPages)
        {
            base.MaxNrOfPages(maxNrOfPages);
            return this;
        }

        /// <summary>
        /// Always add the page number to the generated link for the first page.
        /// </summary>
        /// <remarks>
        /// By default we don't add the page number for page 1 because it results in canonical links.
        /// Use this option to override this behaviour.
        /// </remarks>
        /// <returns></returns>
        public new PagerOptionsBuilder<TModel> AlwaysAddFirstPageNumber()
        {
            base.AlwaysAddFirstPageNumber();
            return this;
        }

        /// <summary>
        /// Set the page routeValue key for pagination links
        /// </summary>
        /// <param name="pageRouteValueKey"></param>
        /// <returns></returns>
        public new PagerOptionsBuilder<TModel> PageRouteValueKey(string pageRouteValueKey)
        {
            if (pageRouteValueKey == null)
            {
                throw new ArgumentException("pageRouteValueKey may not be null", "pageRouteValueKey");
            }
            this.pagerOptions.PageRouteValueKey = pageRouteValueKey;
            return this;
        }

        /// <summary>
        /// Set the text for previous page navigation.
        /// </summary>
        /// <param name="previousPageText"></param>
        /// <returns></returns>
        public new PagerOptionsBuilder<TModel> SetPreviousPageText(string previousPageText)
        {
            base.SetPreviousPageText(previousPageText);
            return this;
        }

        /// <summary>
        /// Set the title for previous page navigation.
        /// </summary>
        /// <param name="previousPageTitle"></param>
        /// <returns></returns>
        public new PagerOptionsBuilder<TModel> SetPreviousPageTitle(string previousPageTitle)
        {
            base.SetPreviousPageTitle(previousPageTitle);
            return this;
        }

        /// <summary>
        /// Set the text for next page navigation.
        /// </summary>
        /// <param name="nextPageText"></param>
        /// <returns></returns>
        public new PagerOptionsBuilder<TModel> SetNextPageText(string nextPageText)
        {
            base.SetNextPageText(nextPageText);
            return this;
        }

        /// <summary>
        /// Set the title for next page navigation.
        /// </summary>
        /// <param name="nextPageTitle"></param>
        /// <returns></returns>
        public new PagerOptionsBuilder<TModel> SetNextPageTitle(string nextPageTitle)
        {
            base.SetNextPageTitle(nextPageTitle);
            return this;
        }

        /// <summary>
        /// Set the text for first page navigation.
        /// </summary>
        /// <param name="firstPageText"></param>
        /// <returns></returns>
        public new PagerOptionsBuilder<TModel> SetFirstPageText(string firstPageText)
        {
            base.SetFirstPageText(firstPageText);
            return this;
        }

        /// <summary>
        /// Set the title for first page navigation.
        /// </summary>
        /// <param name="firstPageTitle"></param>
        /// <returns></returns>
        public new PagerOptionsBuilder<TModel> SetFirstPageTitle(string firstPageTitle)
        {
            base.SetFirstPageTitle(firstPageTitle);
            return this;
        }

        /// <summary>
        /// Set the text for last page navigation.
        /// </summary>
        /// <param name="lastPageText"></param>
        /// <returns></returns>
        public new PagerOptionsBuilder<TModel> SetLastPageText(string lastPageText)
        {
            base.SetLastPageText(lastPageText);
            return this;
        }

        /// <summary>
        /// Set the title for last page navigation.
        /// </summary>
        /// <param name="lastPageTitle"></param>
        /// <returns></returns>
        public new PagerOptionsBuilder<TModel> SetLastPageTitle(string lastPageTitle)
        {
            base.SetLastPageTitle(lastPageTitle);
            return this;
        }

        /// <summary>
        /// Displays first and last navigation pages.
        /// </summary>
        /// <returns></returns>
        public new PagerOptionsBuilder<TModel> DisplayFirstAndLastPage()
        {
            base.DisplayFirstAndLastPage();
            return this;
        }

        /// <summary>
        /// Indicate that the total item count means total page count. This option is for scenario's 
        /// where certain backends don't return the number of total items, but the number of pages.
        /// </summary>
        /// <returns></returns>
        public new PagerOptionsBuilder<TModel> UseItemCountAsPageCount()
        {
            base.UseItemCountAsPageCount();
            return this;
        }
    }
}