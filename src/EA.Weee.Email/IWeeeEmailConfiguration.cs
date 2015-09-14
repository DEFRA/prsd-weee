namespace EA.Weee.Email
{
    using EA.Prsd.Email;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IWeeeEmailConfiguration : IEmailConfiguration
    {
        /// <summary>
        /// The absolute URL for the external user login page.
        /// </summary>
        string ExternalUserLoginUrl { get; }
    }
}
