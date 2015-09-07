using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EA.Prsd.Email
{
    public interface IEmailConfiguration
    {
        /// <summary>
        /// The "from" email address that will be used for all messages.
        /// </summary>
        string SystemEmailAddress { get; }
        
        /// <summary>
        /// When set to true, any email containing a plus character before the at character
        /// will have that portion of the email address removed. This allows multiple unique
        /// email addresses to target a single real address.
        /// For example, the address "john.smith+testuser1@domain.com" would be replaced by
        /// "john.smith@domain.com".
        /// </summary>
        bool TruncateEmailAfterPlus { get; }

        /// <summary>
        /// Whether or not emails will be sent.
        /// </summary>
        bool SendEmail { get; }
    }
}
