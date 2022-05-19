namespace EA.Weee.Web.Services
{
    using System.Web;
    using Infrastructure;

    public class HttpContextService : IHttpContextService
    {
        public string GetAccessToken()
        {
            return HttpContext.Current.User.GetAccessToken();
        }
    }
}