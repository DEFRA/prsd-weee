namespace EA.Weee.Email
{
    using EA.Prsd.Email;

    public interface IWeeeEmailConfiguration : IEmailConfiguration
    {
        /// <summary>
        /// The absolute URL for the external user login page.
        /// </summary>
        string ExternalUserLoginUrl { get; }
    }
}
