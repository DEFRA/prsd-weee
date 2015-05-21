namespace EA.Prsd.Core.Web.ApiClient
{
    using System;
    using System.Net;

    public class ApiBadRequestException : Exception
    {
        public ApiBadRequestException(HttpStatusCode statusCode, ApiBadRequest badRequestData)
        {
            StatusCode = statusCode;
            BadRequestData = badRequestData;
        }

        public HttpStatusCode StatusCode { get; private set; }

        public ApiBadRequest BadRequestData { get; private set; }
    }
}