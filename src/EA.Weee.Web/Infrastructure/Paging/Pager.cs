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
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    
    public class Pager : IHtmlString
    {
        private readonly HtmlHelper htmlHelper;
        private readonly int pageSize;
        private readonly int currentPage;
        private int totalItemCount;
        protected readonly PagerOptions PagerOptions;

        public Pager(HtmlHelper htmlHelper, int pageSize, int currentPage, int totalItemCount)
        {
            this.htmlHelper = htmlHelper;
            this.pageSize = pageSize;
            this.currentPage = currentPage;
            this.totalItemCount = totalItemCount;
            this.PagerOptions = new PagerOptions();
        }

        public Pager Options(Action<PagerOptionsBuilder> buildOptions)
        {
            buildOptions(new PagerOptionsBuilder(this.PagerOptions));
            return this;
        }

        public virtual PaginationModel BuildPaginationModel(Func<int, string> generateUrl)
        {
            int pageCount;
            if (this.PagerOptions.UseItemCountAsPageCount)
            {
                // Set page count directly from total item count instead of calculating. Then calculate totalItemCount based on pageCount and pageSize;
                pageCount = this.totalItemCount;
                this.totalItemCount = pageCount * this.pageSize;
            }
            else
            {
                pageCount = (int)Math.Ceiling(totalItemCount / (double)pageSize);
            }

            var model = new PaginationModel { PageSize = this.pageSize, CurrentPage = this.currentPage, TotalItemCount = this.totalItemCount, PageCount = pageCount };

            // First page
            if (this.PagerOptions.DisplayFirstAndLastPage)
            {
                model.PaginationLinks.Add(new PaginationLink { Active = (currentPage > 1 ? true : false), DisplayText = this.PagerOptions.FirstPageText, DisplayTitle = this.PagerOptions.FirstPageTitle, PageIndex = 1, Url = generateUrl(1) });
            }

            // Previous page
            var previousPageText = this.PagerOptions.PreviousPageText;
            model.PaginationLinks.Add(currentPage > 1 ? new PaginationLink { Active = true, DisplayText = previousPageText, DisplayTitle = this.PagerOptions.PreviousPageTitle, PageIndex = currentPage - 1, Url = generateUrl(currentPage - 1) } : new PaginationLink { Active = false, DisplayText = previousPageText });

            var start = 1;
            var end = pageCount;
            var pagesToDisplay = this.PagerOptions.MaxNrOfPages;

            if (pageCount > pagesToDisplay)
            {
                var middle = (int)Math.Ceiling(pagesToDisplay / 2d) - 1;
                var below = (currentPage - middle);
                var above = (currentPage + middle);

                if (below < 2)
                {
                    above = pagesToDisplay;
                    below = 1;
                }
                else if (above > (pageCount - 2))
                {
                    above = pageCount;
                    below = (pageCount - pagesToDisplay + 1);
                }

                start = below;
                end = above;
            }

            if (start > 1)
            {
                model.PaginationLinks.Add(new PaginationLink { Active = true, PageIndex = 1, DisplayText = "1", Url = generateUrl(1) });
                if (start > 3)
                {
                    model.PaginationLinks.Add(new PaginationLink { Active = true, PageIndex = 2, DisplayText = "2", Url = generateUrl(2) });
                }
                if (start > 2)
                {
                    model.PaginationLinks.Add(new PaginationLink { Active = false, DisplayText = "...", IsSpacer = true });
                }
            }

            for (var i = start; i <= end; i++)
            {
                if (i == currentPage || (currentPage <= 0 && i == 1))
                {
                    model.PaginationLinks.Add(new PaginationLink { Active = true, PageIndex = i, IsCurrent = true, DisplayText = i.ToString() });
                }
                else
                {
                    model.PaginationLinks.Add(new PaginationLink { Active = true, PageIndex = i, DisplayText = i.ToString(), Url = generateUrl(i) });
                }
            }

            if (end < pageCount)
            {
                if (end < pageCount - 1)
                {
                    model.PaginationLinks.Add(new PaginationLink { Active = false, DisplayText = "...", IsSpacer = true });
                }
                if (pageCount - 2 > end)
                {
                    model.PaginationLinks.Add(new PaginationLink { Active = true, PageIndex = pageCount - 1, DisplayText = (pageCount - 1).ToString(), Url = generateUrl(pageCount - 1) });
                }

                model.PaginationLinks.Add(new PaginationLink { Active = true, PageIndex = pageCount, DisplayText = pageCount.ToString(), Url = generateUrl(pageCount) });
            }

            // Next page
            var nextPageText = this.PagerOptions.NextPageText;
            model.PaginationLinks.Add(currentPage < pageCount ? new PaginationLink { Active = true, PageIndex = currentPage + 1, DisplayText = nextPageText, DisplayTitle = this.PagerOptions.NextPageTitle, Url = generateUrl(currentPage + 1) } : new PaginationLink { Active = false, DisplayText = nextPageText });

            // Last page
            if (this.PagerOptions.DisplayFirstAndLastPage)
            {
                model.PaginationLinks.Add(new PaginationLink { Active = (currentPage < pageCount ? true : false), DisplayText = this.PagerOptions.LastPageText, DisplayTitle = this.PagerOptions.LastPageTitle, PageIndex = pageCount, Url = generateUrl(pageCount) });
            }

            model.Options = PagerOptions;
            return model;
        }

        public virtual string ToHtmlString()
        {
            var model = BuildPaginationModel(GeneratePageUrl);

            if (!String.IsNullOrEmpty(this.PagerOptions.DisplayTemplate))
            {
                var templatePath = string.Format("DisplayTemplates/{0}", this.PagerOptions.DisplayTemplate);
                return htmlHelper.Partial(templatePath, model).ToHtmlString();
            }
            else
            {
                var sb = new StringBuilder();

                foreach (var paginationLink in model.PaginationLinks)
                {
                    if (paginationLink.Active)
                    {
                        if (paginationLink.IsCurrent)
                        {
                            sb.AppendFormat("<span class=\"current\">{0}</span>", paginationLink.DisplayText);
                        }
                        else if (!paginationLink.PageIndex.HasValue)
                        {
                            sb.AppendFormat(paginationLink.DisplayText);
                        }
                        else
                        {
                            var linkBuilder = new StringBuilder("<a");

                            linkBuilder.AppendFormat(" href=\"{0}\" title=\"{1}\">{2}</a>", paginationLink.Url, paginationLink.DisplayTitle, paginationLink.DisplayText);

                            sb.Append(linkBuilder.ToString());
                        }
                    }
                    else
                    {
                        if (!paginationLink.IsSpacer)
                        {
                            sb.AppendFormat("<span class=\"disabled\">{0}</span>", paginationLink.DisplayText);
                        }
                        else
                        {
                            sb.AppendFormat("<span class=\"spacer\">{0}</span>", paginationLink.DisplayText);
                        }
                    }
                }
                return sb.ToString();
            }
        }

        protected virtual string GeneratePageUrl(int pageNumber)
        {
            var viewContext = this.htmlHelper.ViewContext;
            var routeDataValues = viewContext.RequestContext.RouteData.Values;
            RouteValueDictionary pageLinkValueDictionary;

            // Avoid canonical errors when pageNumber is equal to 1.
            if (pageNumber == 1 && !this.PagerOptions.AlwaysAddFirstPageNumber)
            {
                pageLinkValueDictionary = new RouteValueDictionary(this.PagerOptions.RouteValues);

                if (routeDataValues.ContainsKey(this.PagerOptions.PageRouteValueKey))
                {
                    routeDataValues.Remove(this.PagerOptions.PageRouteValueKey);
                }
            }
            else
            {
                pageLinkValueDictionary = new RouteValueDictionary(this.PagerOptions.RouteValues) { { this.PagerOptions.PageRouteValueKey, pageNumber } };
            }

            // To be sure we get the right route, ensure the controller and action are specified.
            if (!pageLinkValueDictionary.ContainsKey("controller") && routeDataValues.ContainsKey("controller"))
            {
                pageLinkValueDictionary.Add("controller", routeDataValues["controller"]);
            }

            if (!pageLinkValueDictionary.ContainsKey("action") && routeDataValues.ContainsKey("action"))
            {
                pageLinkValueDictionary.Add("action", routeDataValues["action"]);
            }

            // Fix the dictionary if there are arrays in it.
            pageLinkValueDictionary = pageLinkValueDictionary.FixListRouteDataValues();

            // 'Render' virtual path.
            var virtualPathForArea = RouteTable.Routes.GetVirtualPathForArea(viewContext.RequestContext, pageLinkValueDictionary);

            return virtualPathForArea == null ? null : virtualPathForArea.VirtualPath;
        }
    }
}