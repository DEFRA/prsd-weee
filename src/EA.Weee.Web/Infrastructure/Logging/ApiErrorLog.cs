namespace EA.Weee.Web.Infrastructure.Logging
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Security;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using System.Xml;
    using System.Xml.Serialization;
    using Api.Client;
    using Api.Client.Entities;
    using Elmah;
    using Infrastructure;
    using Prsd.Core.Web.ApiClient;
    using Prsd.Core.Web.OAuth;

    public class ApiErrorLog : ErrorLog
    {
        private readonly IWeeeClient apiClient;
        private readonly IOAuthClientCredentialClient oauthClientCredentialClient;
        private readonly HttpContext httpContext;

        public ApiErrorLog(IDictionary config)
        {
            this.apiClient = DependencyResolver.Current.GetService<IWeeeClient>();
            this.oauthClientCredentialClient = DependencyResolver.Current.GetService<IOAuthClientCredentialClient>();
            this.httpContext = HttpContext.Current;

            ApplicationName = (string)(config["ApplicationName"] ?? string.Empty);
        }

        public async Task<string> GetAccessToken()
        {
            var token = httpContext.User.GetAccessToken();

            if (string.IsNullOrWhiteSpace(token))
            {
                var tokenResponse = await oauthClientCredentialClient.GetClientCredentialsAsync();

                token = tokenResponse.AccessToken;
            }

            return token;
        }

        public override string Log(Error error)
        {
            var errorXml = ErrorXml.EncodeString(error);
            var id = Guid.NewGuid();

            var token = Task.Run(GetAccessToken).Result;

            if (string.IsNullOrWhiteSpace(token) && !httpContext.User.Identity.IsAuthenticated)
            {
                throw new SecurityException("Unauthenticated user");
            }

            var innerException = error.Exception as ApiException;

            if (innerException != null && innerException.ErrorData != null)
            {
                Task.Run(() => apiClient.ErrorLog.Create(token, new ErrorData(Guid.NewGuid(), ApplicationName, error.HostName, error.Type, innerException.Source, innerException.ErrorData.ExceptionMessage,
                    error.User, (int)innerException.StatusCode, error.Time.ToUniversalTime(), GetApiErrorAsXml(innerException.ErrorData))).Result);
            }

            var errorData = new ErrorData(id, ApplicationName, error.HostName, error.Type, error.Source, error.Message,
                error.User,
                error.StatusCode, error.Time.ToUniversalTime(), errorXml);

            var result = Task.Run(() => apiClient.ErrorLog.Create(token, errorData)).Result;

            return id.ToString();
        }

        public override ErrorLogEntry GetError(string id)
        {
            var accessToken = HttpContext.Current.User.GetAccessToken();

            var errorData = Task.Run(() => apiClient.ErrorLog.Get(accessToken, id, ApplicationName)).Result;

            if (errorData == null)
            {
                return null;
            }

            var error = ErrorXml.DecodeString(errorData.ErrorXml);
            return new ErrorLogEntry(this, id, error);
        }

        public override int GetErrors(int pageIndex, int pageSize, IList errorEntryList)
        {
            var accessToken = HttpContext.Current.User.GetAccessToken();

            var errorList = Task.Run(() => apiClient.ErrorLog.GetList(accessToken, pageIndex, pageSize, ApplicationName)).Result;

            foreach (var errorData in errorList.Errors)
            {
                var error = ErrorXml.DecodeString(errorData.ErrorXml);
                errorEntryList.Add(new ErrorLogEntry(this, errorData.Id.ToString(), error));
            }

            return errorList.TotalRecords;
        }

        public string GetApiErrorAsXml(ApiError error)
        {
            var apiError = new XmlSerializer(typeof(ApiError));

            using (var sww = new StringWriter())
            {
                using (var writer = XmlWriter.Create(sww))
                {
                    apiError.Serialize(writer, error);
                    return sww.ToString();
                }
            }
        }
    }
}