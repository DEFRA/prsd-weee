namespace EA.Weee.Api.Identity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines events relating to security that can be audited.
    /// </summary>
    public interface ISecurityEventAuditor
    {
        Task LoginSuccess(string userId);

        Task LoginFailure(string userName);

        Task UserCreated(string userId);

        Task PasswordReset(string userId);

        Task EmailConfirmed(string userId);
    }
}
