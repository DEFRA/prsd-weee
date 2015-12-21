namespace EA.Weee.Web.App_Start
{
    /// <summary>
    /// Provides a mapping for the ReturnUrl parameter used following a successful sign-in.
    /// Some actions do not support HTTP GET. If a user attempts one of these actions when
    /// they are not authenticated (e.g. following a session time out) they are redirected
    /// to the sign-in page and the action's URL is provided in the ReturnUrl parameter.
    /// Following the sign-in, they will possibly receive a 404 page not found error as
    /// the action may have no HTTP GET equivalent at the same URL.
    /// 
    /// This mapping allows an alternative URL to be provided for each action where this
    /// is the case. Returning null from the mapping indicates that no equivalent exists
    /// and that the ReturnUrl parameter should be removed.
    /// </summary>
    public interface IReturnUrlMapping
    {
        /// <summary>
        /// Maps the specified URL to a new URL that should replace the value used by the
        /// ReturnUrl query string parameter.
        /// </summary>
        /// <param name="path">An application absolute path. The path should start with a forward slash
        /// and should include the application name if the site is running under a virtual path.</param>
        /// <returns>Returns the new replacement application absolute path or null if a mapping exists,
        /// otherwise returns the path unchanged.</returns>
        string ApplyMap(string path);

        /// <summary>
        /// Returns true if the specified path is mapped.
        /// </summary>
        /// <param name="path">An application absolute path. The path should start with a forward slash
        /// and should include the application name if the site is running under a virtual path.</param>
        /// <returns>Returns true if the specified path is mapped.</returns>
        bool IsMapped(string path);
    }
}
