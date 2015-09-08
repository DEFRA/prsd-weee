namespace EA.Weee.Web.Authorization
{
    using System.Security.Principal;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public interface IWeeeAuthorization
    {
        Task<LoginResult> SignIn(LoginType loginType, string emailAddress, string password, bool rememberMe);

        void SignOut();
    }
}
