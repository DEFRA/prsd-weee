namespace EA.Weee.Web
{
    using System;
    using System.Web;
    using Elmah;
    using Logging;

    public class Global : HttpApplication
    {
        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Headers.Remove("Server");
        }

        protected void ErrorLog_Filtering(object sender, ExceptionFilterEventArgs args)
        {
            ElmahFilter.FilterSensitiveData(args);
        }

        protected void ErrorMail_Filtering(object sender, ExceptionFilterEventArgs args)
        {
            ElmahFilter.FilterSensitiveData(args);
        }
    }
}