namespace EA.Weee.Web.App_Start
{
    using EA.Prsd.Core;
    using System;
    using System.Collections.Generic;
    using System.Web;

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
    public class ReturnUrlMapping : IReturnUrlMapping
    {
        private Dictionary<string, string> map = new Dictionary<string, string>();

        private string appVirtualPath;

        /// <summary>
        /// Create a new instance of the <see cref="ReturnUrlMapping"/> class, using the
        /// application virtual path defined by HttpRuntime.AppDomainAppVirtualPath.
        /// </summary>
        public ReturnUrlMapping()
        {
            this.appVirtualPath = HttpRuntime.AppDomainAppVirtualPath;
        }

        /// <summary>
        /// Create a new instance of the <see cref="ReturnUrlMapping"/> class, specifying
        /// the application virtal path. This constructor is provided for testing purposes
        /// only. The value must be a relative path starting with a forward slash.
        /// </summary>
        /// <param name="appVirtualPath">The application virtual path.</param>
        public ReturnUrlMapping(string appVirtualPath)
        {
            if (!appVirtualPath.StartsWith("/"))
            {
                throw new ArgumentException("The value of appVirtualPath must start with a forward slash.", "appVirtualPath");
            }

            this.appVirtualPath = appVirtualPath;
        }

        /// <summary>
        /// Specifies a mapping. 
        /// </summary>
        /// <param name="path">The relative path to be mapped. The path should start with a forward slash
        /// and should not include the application name. E.g. "/sign-out".</param>
        /// <param name="mappedPath">The replacement value relative path, or null. Specifying null
        /// will indicate that the ReturnUrl should be removed from the query string.</param>
        public void Add(string path, string mappedPath)
        {
            Guard.ArgumentNotNull(() => path, path);

            if (!path.StartsWith("/"))
            {
                throw new ArgumentException("The value of path must start with a forward slash.", "path");
            }

            string absolutePath = VirtualPathUtility.ToAbsolute("~" + path, appVirtualPath);
            string absoluteMappedPath = null;

            if (mappedPath != null)
            {
                if (!mappedPath.StartsWith("/"))
                {
                    throw new ArgumentException("When specified, the value of mappedPath must start with a forward slash.", "mappedPath");
                }

                absoluteMappedPath = VirtualPathUtility.ToAbsolute("~" + mappedPath, appVirtualPath);
            }

            map.Add(absolutePath, absoluteMappedPath);
        }

        /// <summary>
        /// Maps the specified URL to a new URL that should replace the value used by the
        /// ReturnUrl query string parameter.
        /// </summary>
        /// <param name="path">An application absolute path. The path should start with a forward slash
        /// and should include the application name if the site is running under a virtual path.</param>
        /// <returns>Returns the new replacement application absolute path or null if a mapping exists,
        /// otherwise returns the path unchanged.</returns>
        public string ApplyMap(string path)
        {
            Guard.ArgumentNotNull(() => path, path);

            if (!path.StartsWith("/"))
            {
                throw new ArgumentException("The value of path must start with a forward slash.", "path");
            }

            if (map.ContainsKey(path))
            {
                return map[path];
            }
            else
            {
                return path;
            }
        }

        /// <summary>
        /// Returns true if the specified path is mapped.
        /// </summary>
        /// <param name="path">An application absolute path. The path should start with a forward slash
        /// and should include the application name if the site is running under a virtual path.</param>
        /// <returns>Returns true if the specified path is mapped.</returns>
        public bool IsMapped(string path)
        {
            if (!path.StartsWith("/"))
            {
                throw new ArgumentException("The value of path must start with a forward slash.", "path");
            }

            return map.ContainsKey(path);
        }
    }
}