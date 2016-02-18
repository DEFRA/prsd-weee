namespace EA.Weee.Core.Routing
{
    using System;
    using System.Web;

    [Serializable]
    public class ViewCompetentAuthorityUserRoute : ExternalRoute
    {
        public static readonly string PlaceholderUserId = "placeholderuserid";

        public string CompetentAuthorityUserID { get; set; }

        public ViewCompetentAuthorityUserRoute(string templateUrl)
            : base(templateUrl)
        {
        }

        protected override string ReplacePlaceholders()
        {
            return Url
                .Replace(PlaceholderUserId, HttpUtility.UrlEncode(CompetentAuthorityUserID));
        }
    }
}
