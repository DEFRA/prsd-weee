namespace EA.Weee.Web
{
    using System;
    using System.Web;

    public class Global : HttpApplication
    {
        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Headers.Remove("Server");
        }
    }
}