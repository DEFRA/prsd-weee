namespace EA.Weee.Api.Infrastructure
{
    using System.Net;
    using System.Web.Http;

    public static class ApiControllerExtensions
    {
        public static IHttpActionResult StatusCode(this ApiController controller, HttpStatusCode httpStatusCode,
            object content)
        {
            return new HttpStatusCodeResult(httpStatusCode, content);
        }
    }
}