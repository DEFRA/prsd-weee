namespace EA.Weee.DataAccess.Identity
{
    using Microsoft.AspNet.Identity.EntityFramework;

    public class ApplicationUserStore : UserStore<ApplicationUser>
    {
        public ApplicationUserStore(WeeeIdentityContext context)
            : base(context)
        {
        }
    }
}