namespace EA.Weee.Web.Services
{
    using EA.Weee.Core.Routing;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides resolution of routes that are used externally to the WEEE website.
    /// </summary>
    public interface IExternalRouteService
    {
        /// <summary>
        /// The absolute URL of the user account activation page for an internal user.
        /// </summary>
        string ActivateInternalUserAccountUrl { get; }

        /// <summary>
        /// The absolute URL of the user account activation page for an external user.
        /// </summary>
        string ActivateExternalUserAccountUrl { get; }

        /// <summary>
        /// The route to the password reset page for an internal user.
        /// </summary>
        ResetPasswordRoute InternalUserResetPasswordRoute { get; }

        /// <summary>
        /// The route to the password reset page for an external user.
        /// </summary>
        ResetPasswordRoute ExternalUserResetPasswordRoute { get; }
    }
}
