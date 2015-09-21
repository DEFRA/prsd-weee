namespace EA.Weee.Web.App_Start
{
    using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Routing;

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
        public PathString AdminLoginPath { get; set; }

        public WeeeCookieAuthenticationProvider()
        {
            AdminAreaName = "admin";
            AdminLoginPath = new PathString("/admin/account/sign-in");

            // Add our custom login to the redirect before applying the deafult implementation.
            OnApplyRedirect = (context) =>
            {
                UpdateRedirectUrlToAdminLoginPageIfNecessary(context);
                ApplyReturnUrlMapping(context);
                defaultImplementation.ApplyRedirect(context);
            };
        }

        private void UpdateRedirectUrlToAdminLoginPageIfNecessary(CookieApplyRedirectContext context)
        {
            if (UserWasAccessingInternalArea(context))
            {
                string standardLoginPathString = context.Options.LoginPath.Value;
                string adminLoginPathString = AdminLoginPath.Value;

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
        private bool UserWasAccessingInternalArea(CookieApplyRedirectContext context)
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
        /// This dictionary defines the mapping used by the <see cref="ApplyReturnUrlMapping"/> method below.
        /// A null value will result in the ReturnUrl parameter being removed from the query string.
        /// </summary>
        private static readonly Dictionary<string, string> returnUrlMapping = new Dictionary<string, string>()
        {
            { "/account/sign-out", null },
            { "/admin/account/sign-out", null }
        };

        /// <summary>
        /// If a user times out before attempting an action, the "ReturnUrl" query string
        /// parameter included in the sign-in page URL may need to be rewritten.
        /// This can be used to prevent the user being redirected with a GET to a POST-only action
        /// after they sign back in.
        /// </summary>
        private void ApplyReturnUrlMapping(CookieApplyRedirectContext context)
        {
            Uri currentUri = new Uri(context.RedirectUri);
            var queryStringParameters = HttpUtility.ParseQueryString(currentUri.Query);

            string returnUrl = queryStringParameters["ReturnUrl"];

            if (returnUrlMapping.ContainsKey(returnUrl))
            {
                returnUrl = returnUrlMapping[returnUrl];

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