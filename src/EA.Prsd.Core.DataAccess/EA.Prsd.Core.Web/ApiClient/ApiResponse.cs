namespace EA.Prsd.Core.Web.ApiClient
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Net;

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass",
        Justification = "File contains generic and non-generic of the same class.")]
    public class ApiResponse<T> : ApiResponse
    {
        public ApiResponse(HttpStatusCode statusCode, T result)
            : this(statusCode, null)
        {
            Result = result;
        }

        public ApiResponse(HttpStatusCode statusCode, IEnumerable<string> errors)
            : base(statusCode, errors)
        {
        }

        public T Result { get; private set; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass",
        Justification = "File contains generic and non-generic of the same class.")]
    public class ApiResponse
    {
        private readonly HttpStatusCode statusCode;

        public ApiResponse(HttpStatusCode statusCode) 
            : this(statusCode, null)
        {
        }

        public ApiResponse(HttpStatusCode statusCode, IEnumerable<string> errors)
        {
            this.statusCode = statusCode;
            Errors = errors ?? Enumerable.Empty<string>();
        }

        public IEnumerable<string> Errors { get; private set; }

        public bool HasErrors
        {
            get { return Errors.Any(); }
        }

        public HttpStatusCode StatusCode
        {
            get { return statusCode; }
        }
    }
}