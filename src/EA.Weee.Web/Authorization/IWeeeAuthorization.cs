namespace EA.Weee.Web.Authorization
{
    using System.Threading.Tasks;

    public interface IWeeeAuthorization
    {
        Task<AuthorizationState> GetAuthorizationState();

        Task<LoginResult> SignIn(string emailAddress, string password, bool rememberMe);

        void SignOut();
    }
}
