namespace EA.Weee.Api.IdSrv
{
    using DataAccess.Identity;
    using Identity;

    public class UserService : AspNetIdentityUserService<ApplicationUser, string>
    {
        public UserService(ApplicationUserManager userMgr)
            : base(userMgr)
        {
        }
    }
}