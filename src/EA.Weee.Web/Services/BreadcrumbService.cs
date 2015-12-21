namespace EA.Weee.Web.Services
{
    using EA.Weee.Core.Scheme;

    public class BreadcrumbService
    {
        /// <summary>
        /// For an external user, the organisation currently in scope.
        /// </summary>
        public string ExternalOrganisation { get; set; }

        /// <summary>
        /// For an external user, the activity currently in scope.
        /// </summary>
        public string ExternalActivity { get; set; }

        /// <summary>
        /// For an internal user, the activity currently in scope.
        /// </summary>
        public string InternalActivity { get; set; }

        /// <summary>
        /// For an internal user, the organisation currently in scope.
        /// </summary>
        public string InternalOrganisation { get; set; }

        /// <summary>
        /// For an internal user, the user currently in scope.
        /// </summary>
        public string InternalUser { get; set; }

        /// <summary>
        /// Information about the scheme currently in scope.
        /// </summary>
        public SchemePublicInfo SchemeInfo { get; set; }

        /// <summary>
        /// The activity currently in scope when accessing the test area.
        /// </summary>
        public string TestAreaActivity { get; set; }
    }
}