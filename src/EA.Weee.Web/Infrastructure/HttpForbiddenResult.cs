namespace EA.Weee.Web.Infrastructure
{
    using System.Net;
    using System.Web.Mvc;

    public class HttpForbiddenResult : HttpStatusCodeResult
    {
        public HttpForbiddenResult()
            : base(HttpStatusCode.Forbidden)
        {
        }

        public HttpForbiddenResult(string statusDescription)
            : base(HttpStatusCode.Forbidden, statusDescription)
        {
        }
    }
}