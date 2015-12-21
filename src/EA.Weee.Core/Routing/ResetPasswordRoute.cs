namespace EA.Weee.Core.Routing
{
    using System;
    using System.Web;

    /// <summary>
    /// Defines the route to a reset password page, using placeholders
    /// for the user Id and password reset tokens.
    /// </summary>
    [Serializable]
    public class ResetPasswordRoute : ExternalRoute
    {
        public static readonly string PlaceholderUserId = "placeholderuserid";
        public static readonly string PlaceholderToken = "placeholdertoken";

        public string UserID { get; set; }
        public string Token { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResetPasswordRoute"/> class.
        /// </summary>
        /// <param name="templateUrl">
        /// The absolute URL to the password reset page.
        /// The URL can include two placeholders which will be replaced by real values.
        /// The placeholder strings are defined by static properties on this class.
        /// </param>
        public ResetPasswordRoute(string templateUrl)
            : base(templateUrl)
        {
        }

        protected override string ReplacePlaceholders()
        {
            return Url
                .Replace(PlaceholderUserId, HttpUtility.UrlEncode(UserID))
                .Replace(PlaceholderToken, HttpUtility.UrlEncode(Token));
        }
    }
}
