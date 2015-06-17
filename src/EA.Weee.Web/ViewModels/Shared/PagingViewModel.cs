namespace EA.Weee.Web.ViewModels.Shared
{
    using System;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    public class PagingViewModel
    {
        public int TotalPages { get; private set; }

        public int PreviousPage { get; private set; }

        public int NextPage { get; private set; }

        public int StartingAt { get; private set; }

        public string PreviousPageLink { get; private set; }

        public string NextPageLink { get; private set; }

        public PagingViewModel(
            int totalPages,
            int previousPage,
            int nextPage,
            int startingAt,
            string actionName,
            string controllerName,
            object routeValues = null)
        {
            TotalPages = totalPages;
            PreviousPage = previousPage;
            NextPage = nextPage;
            StartingAt = startingAt;
            PreviousPageLink = MakePageLink(PreviousPage, actionName, controllerName, routeValues);
            NextPageLink = MakePageLink(NextPage, actionName, controllerName, routeValues);
        }

        public PagingViewModel(string controllerName, string actionName, object routeValues = null)
            : this(1, 0, 2, 1, controllerName, actionName, routeValues)
        {
        }

        public static PagingViewModel FromValues(
            int numberOfItems,
            int itemsPerPage,
            int currentPage,
            string actionName,
            string controllerName,
            object routeValues = null)
        {
            var totalPages = (int)Math.Ceiling(((double)numberOfItems / (double)itemsPerPage));
            var previousPage = currentPage - 1;
            var nextPage = currentPage + 1;
            var startingAt = ((currentPage - 1) * itemsPerPage) + 1;

            return new PagingViewModel(
                totalPages,
                previousPage,
                nextPage,
                startingAt,
                actionName,
                controllerName,
                routeValues);
        }

        private string MakePageLink(int page, string actionName, string controllerName, object routeValues)
        {
            var url = new UrlHelper(HttpContext.Current.Request.RequestContext).Action(
                actionName,
                controllerName,
                routeValues);

            var path = string.Join(string.Empty, url.Split('?').Take(1));
            var queryString = string.Join(string.Empty, url.Split('?').Skip(1));

            var nameValues = HttpUtility.ParseQueryString(queryString);
            nameValues.Set("Page", page.ToString());

            return path + "?" + nameValues.ToString();
        }
    }
}