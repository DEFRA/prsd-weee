namespace EA.Weee.Web.Infrastructure.Logging
{
    using System;
    using System.Collections;
    using System.Security;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Api.Client;
    using Api.Client.Entities;
    using Elmah;
    using Filters;
    using Infrastructure;
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

            var token = AsyncHelpers.RunSync(GetAccessToken);

            if (string.IsNullOrWhiteSpace(token) && !httpContext.User.Identity.IsAuthenticated)
            {
                throw new SecurityException("Unauthenticated user");
            }

            var errorData = new ErrorData(id, ApplicationName, error.HostName, error.Type, error.Source, error.Message,
                error.User,
                error.StatusCode, error.Time.ToUniversalTime(), errorXml);

            AsyncHelpers.RunSync(() => apiClient.ErrorLog.Create(token, errorData));

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
    }
}