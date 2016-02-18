namespace EA.Weee.Security
{
    using System.Data.Entity;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using DataAccess;
    using Prsd.Core.Domain;

    public class RoleRequestHandler : IRoleBasedResponseHandler
    {
        private readonly WeeeContext context;
        private readonly IUserContext userContext;

        public RoleRequestHandler(IUserContext userContext, WeeeContext context)
        {
            this.context = context;
            this.userContext = userContext;
        }

        public async Task<object> HandleAsync(object response)
        {
            var roleBasedResponse = response as RoleBasedResponse;
            if (roleBasedResponse != null)
            {
                var claimsIdentity = userContext.Principal.Identity as ClaimsIdentity;

                if (claimsIdentity != null && claimsIdentity.HasClaim(ClaimTypes.AuthenticationMethod, Claims.CanAccessInternalArea))
                {
                    var internalUser = await context.CompetentAuthorityUsers
                        .Include(cau => cau.Role)
                        .SingleOrDefaultAsync(cau => cau.UserId == userContext.UserId.ToString());

                    if (internalUser != null)
                    {
                        roleBasedResponse.AddUserRole(new Role { Description = internalUser.Role.Description, Name = internalUser.Role.Name });
                    }
                }
            }

            return response;
        }
    }
}
