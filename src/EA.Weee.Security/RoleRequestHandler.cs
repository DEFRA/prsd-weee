namespace EA.Weee.Security
{
    using System.Data.Entity;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using DataAccess;
    using Prsd.Core.Domain;

    public class RoleRequestHandler : IRoleRequestHandler
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
            if (!response.GetType().IsSubclassOf(typeof(RoleBasedResponse)))
            {
                await Task.Yield();
                return response;
            }

            var roleBasedResponse = (RoleBasedResponse)response;
            var claimsIdentity = userContext.Principal.Identity as ClaimsIdentity;

            if (claimsIdentity != null && claimsIdentity.HasClaim(ClaimTypes.AuthenticationMethod, Claims.CanAccessInternalArea))
            {
                var internalUser = await context.CompetentAuthorityUsers
                    .Include(cau => cau.Role)
                    .SingleOrDefaultAsync(cau => cau.UserId == userContext.UserId.ToString());

                if (internalUser != null && internalUser.Role != null)
                {
                    roleBasedResponse.AddUserRole(new Role { Description = internalUser.Role.Description, Name = internalUser.Role.Name });
                }
            }

            return response;
        }
    }
}
