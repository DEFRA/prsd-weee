namespace EA.Weee.Web
{
    using EA.Weee.Web.App_Start;
    using Elmah;
    using Logging;
    using System;
    using System.Web;

    public class Global : HttpApplication
    {
        protected void Application_Start()
        {
            CustomModelBindersConfig.RegisterCustomModelBinders();
        }

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