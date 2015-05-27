namespace EA.Weee.Api.Controllers
{
    using System;
    using System.Net;
    using System.Security;
    using System.Security.Authentication;
    using System.Threading.Tasks;
    using System.Web.Http;
    using EA.Prsd.Core.Mediator;
    using EA.Prsd.Core.Web.ApiClient;
    using Elmah;
    using Infrastructure;
    using Newtonsoft.Json;

    public class MediatorController : ApiController
    {
        private readonly IMediator mediator;

        public MediatorController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        /// <summary>
        ///     This API action fills the role of the mediator. It receives messages and sends responses.
        /// </summary>
        /// <param name="apiRequest">The wrapped request object with assembly qualified type name.</param>
        /// <returns>An object of the correct type (which will be serialized as json).</returns>
        [HttpPost]
        [Route("Send")]
        public async Task<IHttpActionResult> Send(ApiRequest apiRequest)
        {
            var typeInformation = new RequestTypeInformation(apiRequest);

            var result = JsonConvert.DeserializeObject(apiRequest.RequestJson, typeInformation.RequestType);

            try
            {
                var response = await mediator.SendAsync(result, typeInformation.ResponseType);
                return Ok(response);
            }
            catch (AuthenticationException ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return this.StatusCode(HttpStatusCode.Unauthorized, new HttpError(ex, includeErrorDetail: true));
            }
            catch (SecurityException ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return this.StatusCode(HttpStatusCode.Forbidden, new HttpError(ex, includeErrorDetail: true));
            }
        }

        private class RequestTypeInformation
        {
            public RequestTypeInformation(ApiRequest apiRequest)
            {
                RequestType = Type.GetType(apiRequest.TypeName);

                if (RequestType == null)
                {
                    throw new InvalidOperationException(
                        "The passed type name could not be resolved to a type! Type name: " + apiRequest.TypeName);
                }

                ResponseType = RequestType.GetInterfaces()[0].GenericTypeArguments[0];
            }

            public Type RequestType { get; private set; }

            public Type ResponseType { get; private set; }
        }
    }
}