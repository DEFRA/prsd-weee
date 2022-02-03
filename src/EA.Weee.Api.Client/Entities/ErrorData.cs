namespace EA.Weee.Api.Client.Entities
{
    using System;

    public class ErrorData
    {
        public Guid Id { get; set; }

        public string ApplicationName { get; set; }

        public string HostName { get; set; }

        public string Type { get; set; }

        public string Source { get; set; }

        public string Message { get; set; }

        public string User { get; set; }

        public int StatusCode { get; set; }

        public DateTime Date { get; set; }

        public string ErrorXml { get; set; }

        public ErrorData(Guid id, string applicationName, string hostName, string type, string source, string message, string user, int statusCode, DateTime date, string errorXml)
        {
            Id = id;
            ApplicationName = applicationName;
            HostName = hostName;
            Type = type;
            Source = source;
            Message = message;
            User = user;
            StatusCode = statusCode;
            Date = date;
            ErrorXml = errorXml;
        }

        public ErrorData()
        {
        }
    }
}