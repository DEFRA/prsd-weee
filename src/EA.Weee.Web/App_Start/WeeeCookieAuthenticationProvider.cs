namespace EA.Weee.Web.App_Start
{
    using System;
    using System.IO;
    using System.Net;
    using System.Web;
    using System.Web.Routing;
    using Microsoft.Owin.Security.Cookies;
    using Prsd.Core.Web.Mvc.Owin;

    /// <summary>
    /// This cookie authentication provider overrides the default "redirect to login page" behaviour.
    /// If the URI of the requested resource indicates that it is within the admin area of the website,
    /// the user will be redirected to the admin login page page rather than the standard login page.
    /// </summary>
    public class WeeeCookieAuthenticationProvider : PrsdCookieAuthenticationProvider
    {
        /// <summary>
        /// Because the provider is implemented as a set of Funcs rather than methods, we cannot
        /// derive from the base CookieAuthenticationProvider class to make use of the default
        /// implementation. Therefore we need to create an instance and wrap the Funcs in
        /// composite method calls.
        /// </summary>
        private readonly CookieAuthenticationProvider defaultImplementation = new CookieAuthenticationProvider();

        /// <summary>
        /// The AdminAreaName property informs the provider of name of the admin area.
        /// This property defaults to "Admin".
        /// </summary>
        public string AdminAreaName { get; set; }

        /// <summary>
        /// The AdminLoginPath property informs the provider where to redirect unauthenticated users 
        /// trying to access resources within the admin area.
        /// This property defaults to "/Admin/Account/SignIn".
        /// </summary>
        public string AdminLoginPath { get; set; }

        private readonly IReturnUrlMapping returnUrlMapping;

        public WeeeCookieAuthenticationProvider(IReturnUrlMapping returnUrlMapping)
        {
            this.returnUrlMapping = returnUrlMapping;

            AdminAreaName = "admin";
            AdminLoginPath = "/admin/account/sign-in";

            OnValidateIdentity = context => IdentityValidationHelper.OnValidateIdentity(context);

            // Add our custom login to the redirect before applying the deafult implementation.
            OnApplyRedirect = (context) =>
            {
                ErrorIfAlreadyAuthenticated(context);
                UpdateRedirectUrlToAdminLoginPageIfNecessary(context);
                ApplyReturnUrlMapping(context);
                defaultImplementation.ApplyRedirect(context);
            };
        }

        private void ErrorIfAlreadyAuthenticated(CookieApplyRedirectContext context)
        {
            if (context.Request.User.Identity.IsAuthenticated)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            }
        }

        public void UpdateRedirectUrlToAdminLoginPageIfNecessary(CookieApplyRedirectContext context)
        {
            if (UserWasAccessingInternalArea(context))
            {
                string standardLoginPathString = VirtualPathUtility.ToAbsolute(context.Options.LoginPath.Value);
                string adminLoginPathString = VirtualPathUtility.ToAbsolute(AdminLoginPath);

                Uri currentUri = new Uri(context.RedirectUri);
                
                UriBuilder uriBuilder = new UriBuilder(currentUri);
                
                if (string.Equals(uriBuilder.Path, standardLoginPathString, StringComparison.OrdinalIgnoreCase))
                {
                    uriBuilder.Path = adminLoginPathString;
                }

                context.RedirectUri = uriBuilder.Uri.ToString();
            }
        }

        /// <summary>
        /// Parse the request URI to determine whether the user was trying to access the internal area.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool UserWasAccessingInternalArea(CookieApplyRedirectContext context)
        {
            HttpRequest request = new HttpRequest(null, context.Request.Uri.OriginalString, context.Request.Uri.Query);
            HttpResponse response = new HttpResponse(new StringWriter());
            HttpContext httpContext = new HttpContext(request, response);

            var routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));

            string area = routeData.DataTokens["area"] as string;

            bool userWasAccessingInternalArea = string.Equals(area, AdminAreaName, StringComparison.OrdinalIgnoreCase);
            
            return userWasAccessingInternalArea;
        }

        /// <summary>
        /// If a user times out before attempting an action, the "ReturnUrl" query string
        /// parameter included in the sign-in page URL may need to be rewritten.
        /// This can be used to prevent the user being redirected with a GET to a POST-only action
        /// after they sign back in.
        /// </summary>
        public void ApplyReturnUrlMapping(CookieApplyRedirectContext context)
        {
            Uri currentUri = new Uri(context.RedirectUri);
            var queryStringParameters = HttpUtility.ParseQueryString(currentUri.Query);

            string returnUrl = queryStringParameters["ReturnUrl"];

            if (returnUrlMapping.IsMapped(returnUrl))
            {
                returnUrl = returnUrlMapping.ApplyMap(returnUrl);

                if (returnUrl != null)
                {
                    queryStringParameters["ReturnUrl"] = returnUrl;
                }
                else
                {
                    queryStringParameters.Remove("ReturnUrl");
                }

                UriBuilder uriBuilder = new UriBuilder(currentUri);
                uriBuilder.Query = queryStringParameters.ToString();
                context.RedirectUri = uriBuilder.Uri.ToString();
            }
        }
    }
}