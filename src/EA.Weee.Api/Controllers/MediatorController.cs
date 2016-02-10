namespace EA.Weee.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
    using Security;

    [RoutePrefix("api")]
    public class MediatorController : ApiController
    {
        private readonly IMediator mediator;
        private readonly IRoleRequestHandler roleRequestHandler;

        public MediatorController(IMediator mediator, IRoleRequestHandler roleRequestHandler)
        {
            this.mediator = mediator;
            this.roleRequestHandler = roleRequestHandler;
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
            if (!ModelState.IsValid)
            {
                // We need to check here that the ModelState is valid. Some infrastructury low-level
                // wizardry may have caused the apiRequest parameter to be provided as null.
                // For example, if the JSON request exceeds the maximum request size.
                IEnumerable<string> errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => (e.Exception != null) ? e.Exception.Message : e.ErrorMessage);

                string message = string.Join(" ", errorMessages);

                throw new Exception(message);
            }
            
            var typeInformation = new RequestTypeInformation(apiRequest);

            var result = JsonConvert.DeserializeObject(apiRequest.RequestJson, typeInformation.RequestType);

            try
            {
                var response = await mediator.SendAsync(result, typeInformation.ResponseType);
                response = await roleRequestHandler.HandleAsync(response);
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