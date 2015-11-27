namespace EA.Weee.Api.Identity
{
    using System.Threading.Tasks;
    using Microsoft.AspNet.Identity;

    /// <summary>
    /// Defines events relating to security that can be audited.
    /// </summary>
    public interface ISecurityEventAuditor
    {
        Task LoginSuccess(string userId);

        Task LoginFailure(string userName);

        Task UserCreated(IUser<string> user);

        Task PasswordReset(string userId);

        Task EmailConfirmed(string userId);

        Task UserUpdated(string userId, IUser<string> user);
    }
}
