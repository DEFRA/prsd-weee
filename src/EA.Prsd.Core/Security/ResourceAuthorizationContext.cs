namespace EA.Prsd.Core.Security
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;

    // https://github.com/IdentityModel/Thinktecture.IdentityModel/blob/master/source/Owin.ResourceAuthorization/ResourceAuthorizationContext.cs
    public class ResourceAuthorizationContext
    {
        public IEnumerable<Claim> Action { get; set; }
        public IEnumerable<Claim> Resource { get; set; }
        public ClaimsPrincipal Principal { get; set; }

        public ResourceAuthorizationContext(ClaimsPrincipal principal, IEnumerable<Claim> action, IEnumerable<Claim> resource)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            if (resource == null)
            {
                throw new ArgumentNullException("resource");
            }

            if (principal == null)
            {
                throw new ArgumentNullException("principal");
            }

            Action = action;
            Resource = resource;
            Principal = principal;
        }

        public ResourceAuthorizationContext(ClaimsPrincipal principal, string action, params string[] resource)
        {
            if (string.IsNullOrWhiteSpace(action))
            {
                throw new ArgumentNullException("action");
            }

            if (resource == null || resource.Length == 0)
            {
                throw new ArgumentNullException("resource");
            }

            if (principal == null)
            {
                throw new ArgumentNullException("principal");
            }

            Action = new List<Claim> { new Claim("name", action) };
            Resource = new List<Claim>(from r in resource select new Claim("name", r));
            Principal = principal;
        }
    }
}