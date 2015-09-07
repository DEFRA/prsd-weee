namespace EA.Weee.Web.Services
{
    using System.Security.Principal;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public interface IWeeeAuthorization
    {
        Task<ActionResult> SignIn(string emailAddress, string password, bool rememberMe, string returnUrl = "");

        ActionResult SignOut(IPrincipal user);
    }
}
