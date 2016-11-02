namespace EA.Weee.Api.IdSrv
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web;
    using Autofac;
    using Autofac.Integration.Owin;
    using Identity;
    using IdentityModel.Owin.ClaimsTransformation;

    internal static class ClaimsTransformationOptionsFactory
    {
        public static ClaimsTransformationOptions Create()
        {
            return new ClaimsTransformationOptions
            {
                ClaimsTransformation = TransformClaims
            };
        }

        private static async Task<ClaimsPrincipal> TransformClaims(ClaimsPrincipal incoming)
        {
            if (!incoming.Identity.IsAuthenticated)
            {
                return incoming;
            }

            var context = HttpContext.Current.GetOwinContext().GetAutofacLifetimeScope();

            var manager = context.Resolve<ApplicationUserManager>();

            var user = await manager.FindByIdAsync(incoming.FindFirst("sub").Value);
            var identity = await manager.CreateIdentityAsync(user, "API");

            return new ClaimsPrincipal(identity);
        }
    }
}