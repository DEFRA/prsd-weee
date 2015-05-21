namespace EA.Prsd.Core.Web.ApiClient
{
    using System;
    using System.Net;

    public class ApiException : Exception
    {
        public ApiException(HttpStatusCode statusCode, ApiError errorData)
        {
            StatusCode = statusCode;
            ErrorData = errorData;
        }

        public HttpStatusCode StatusCode { get; private set; }

        public ApiError ErrorData { get; private set; }
    }
}