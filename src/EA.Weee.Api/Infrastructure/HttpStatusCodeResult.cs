namespace EA.Weee.Api.Infrastructure
{
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;

    public class HttpStatusCodeResult : IHttpActionResult
    {
        private readonly object content;
        private readonly HttpStatusCode statusCode;

        public HttpStatusCodeResult(HttpStatusCode statusCode, object content)
        {
            this.statusCode = statusCode;
            this.content = content;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(statusCode)
            {
                Content = new ObjectContent(content.GetType(), content, new JsonMediaTypeFormatter())
            };
            return Task.FromResult(response);
        }
    }
}