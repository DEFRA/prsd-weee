namespace EA.Weee.Requests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The result of an update request.
    /// </summary>
    public enum UpdateResult
    {
        /// <summary>
        /// The update was successful.
        /// </summary>
        OK,

        /// <summary>
        /// The update encountered a concurrecny exception.
        /// </summary>
        ConcurrencyError,

        /// <summary>
        /// An unknown error occured during the update.
        /// </summary>
        UnknownError
    }
}
