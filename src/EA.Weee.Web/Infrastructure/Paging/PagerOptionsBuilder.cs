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
    using System.Web.Routing;

    /// <summary>
    /// Pager options builder class. Enables a fluent interface for adding options to the pager.
    /// </summary>
    public class PagerOptionsBuilder
    {
        protected PagerOptions pagerOptions;

        public PagerOptionsBuilder(PagerOptions pagerOptions)
        {
            this.pagerOptions = pagerOptions;
        }

        /// <summary>
        /// Set the action name for the pager links. Note that we're always using the current controller.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public PagerOptionsBuilder Action(string action)
        {
            if (action != null)
            {
                if (pagerOptions.RouteValues.ContainsKey("action"))
                {
                    throw new ArgumentException("The valuesDictionary already contains an action.", "action");
                }
                pagerOptions.RouteValues.Add("action", action);
                pagerOptions.Action = action;
            }
            return this;
        }

        /// <summary>
        /// Add a custom route value parameter for the pager links.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public PagerOptionsBuilder AddRouteValue(string name, object value)
        {
            pagerOptions.RouteValues[name] = value;
            return this;
        }

        /// <summary>
        /// Set the text for previous page navigation.
        /// </summary>
        /// <param name="previousPageText"></param>
        /// <returns></returns>
        public PagerOptionsBuilder SetPreviousPageText(string previousPageText)
        {
            pagerOptions.PreviousPageText = previousPageText;
            return this;
        }

        /// <summary>
        /// Set the title for previous page navigation.
        /// </summary>
        /// <param name="previousPageTitle"></param>
        /// <returns></returns>
        public PagerOptionsBuilder SetPreviousPageTitle(string previousPageTitle)
        {
            pagerOptions.PreviousPageTitle = previousPageTitle;
            return this;
        }

        /// <summary>
        /// Set the text for next page navigation.
        /// </summary>
        /// <param name="nextPageText"></param>
        /// <returns></returns>
        public PagerOptionsBuilder SetNextPageText(string nextPageText)
        {
            pagerOptions.NextPageText = nextPageText;
            return this;
        }

        /// <summary>
        /// Set the title for next page navigation.
        /// </summary>
        /// <param name="nextPageTitle"></param>
        /// <returns></returns>
        public PagerOptionsBuilder SetNextPageTitle(string nextPageTitle)
        {
            pagerOptions.NextPageTitle = nextPageTitle;
            return this;
        }

        /// <summary>
        /// Set the text for first page navigation.
        /// </summary>
        /// <param name="firstPageText"></param>
        /// <returns></returns>
        public PagerOptionsBuilder SetFirstPageText(string firstPageText)
        {
            pagerOptions.FirstPageText = firstPageText;
            return this;
        }

        /// <summary>
        /// Set the title for first page navigation.
        /// </summary>
        /// <param name="firstPageTitle"></param>
        /// <returns></returns>
        public PagerOptionsBuilder SetFirstPageTitle(string firstPageTitle)
        {
            pagerOptions.FirstPageTitle = firstPageTitle;
            return this;
        }

        /// <summary>
        /// Set the text for last page navigation.
        /// </summary>
        /// <param name="lastPageText"></param>
        /// <returns></returns>
        public PagerOptionsBuilder SetLastPageText(string lastPageText)
        {
            pagerOptions.LastPageText = lastPageText;
            return this;
        }

        /// <summary>
        /// Set the title for last page navigation.
        /// </summary>
        /// <param name="lastPageTitle"></param>
        /// <returns></returns>
        public PagerOptionsBuilder SetLastPageTitle(string lastPageTitle)
        {
            pagerOptions.LastPageTitle = lastPageTitle;
            return this;
        }

        /// <summary>
        /// Displays first and last navigation pages.
        /// </summary>
        /// <returns></returns>
        public PagerOptionsBuilder DisplayFirstAndLastPage()
        {
            pagerOptions.DisplayFirstAndLastPage = true;
            return this;
        }

        /// <summary>
        /// Set custom route value parameters for the pager links.
        /// </summary>
        /// <param name="routeValues"></param>
        /// <returns></returns>
        public PagerOptionsBuilder RouteValues(object routeValues)
        {
            RouteValues(new RouteValueDictionary(routeValues));
            return this;
        }

        /// <summary>
        /// Set custom route value parameters for the pager links.
        /// </summary>
        /// <param name="routeValues"></param>
        /// <returns></returns>
        public PagerOptionsBuilder RouteValues(RouteValueDictionary routeValues)
        {
            if (routeValues == null)
            {
                throw new ArgumentException("routeValues may not be null", "routeValues");
            }
            this.pagerOptions.RouteValues = routeValues;
            if (!string.IsNullOrWhiteSpace(pagerOptions.Action) && !pagerOptions.RouteValues.ContainsKey("action"))
            {
                pagerOptions.RouteValues.Add("action", pagerOptions.Action);
            }
            return this;
        }

        /// <summary>
        /// Set the name of the DisplayTemplate view to use for rendering.
        /// </summary>
        /// <param name="displayTemplate"></param>
        /// <remarks>The view must have a model of IEnumerable&lt;PaginationModel&gt;</remarks>
        /// <returns></returns>
        public PagerOptionsBuilder DisplayTemplate(string displayTemplate)
        {
            this.pagerOptions.DisplayTemplate = displayTemplate;
            return this;
        }

        /// <summary>
        /// Set the maximum number of pages to show. The default is 10.
        /// </summary>
        /// <param name="maxNrOfPages"></param>
        /// <returns></returns>
        public PagerOptionsBuilder MaxNrOfPages(int maxNrOfPages)
        {
            this.pagerOptions.MaxNrOfPages = maxNrOfPages;
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
        public PagerOptionsBuilder AlwaysAddFirstPageNumber()
        {
            this.pagerOptions.AlwaysAddFirstPageNumber = true;
            return this;
        }

        /// <summary>
        /// Set the page routeValue key for pagination links
        /// </summary>
        /// <param name="pageRouteValueKey"></param>
        /// <returns></returns>
        public PagerOptionsBuilder PageRouteValueKey(string pageRouteValueKey)
        {
            if (pageRouteValueKey == null)
            {
                throw new ArgumentException("pageRouteValueKey may not be null", "pageRouteValueKey");
            }
            this.pagerOptions.PageRouteValueKey = pageRouteValueKey;
            return this;
        }

        /// <summary>
        /// Indicate that the total item count means total page count. This option is for scenario's 
        /// where certain backends don't return the number of total items, but the number of pages.
        /// </summary>
        /// <returns></returns>
        public PagerOptionsBuilder UseItemCountAsPageCount()
        {
            this.pagerOptions.UseItemCountAsPageCount = true;
            return this;
        }
    }
}