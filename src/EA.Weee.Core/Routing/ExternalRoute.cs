namespace EA.Weee.Core.Routing
{
    using System;

    /// <summary>
    /// Represents a route that can be expressed as an absolute URL outside of
    /// the context of the website that owns the route.
    /// 
    /// Classes deriving from <see cref="ExternalRoute"/> may define placeholder
    /// values which will be replaced with actual values at the time the absolute URL
    /// is generated. Care must be taken to ensure that any formatting rules
    /// applied to URLs do not interfere with the values chosen for placeholders.
    /// 
    /// To allow instances of <see cref="ExternalRoute"/> to be useful outside of
    /// the website context, all derived classes must be marked as [Serializable].
    /// </summary>
    [Serializable]
    public class ExternalRoute
    {
        protected readonly string Url;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalRoute"/> class.
        /// </summary>
        /// <param name="url">
        /// The absolute URL to the page.
        /// </param>
        public ExternalRoute(string url)
        {
            Url = url;
        }

        /// <summary>
        /// Generates the absolute URL by replacing any placeholders with
        /// actual values.
        /// </summary>
        /// <returns></returns>
        public string GenerateUrl()
        {
            return ReplacePlaceholders();
        }

        /// <summary>
        /// Returns a string with any placeholders replaced with actual values.
        /// All values should be URL encoded if required.
        /// </summary>
        /// <returns></returns>
        protected virtual string ReplacePlaceholders()
        {
            return Url;
        }
    }
}
