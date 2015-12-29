namespace EA.Weee.RequestHandlers.Charges.IssuePendingCharges
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Domain.Charges;
    using Domain.Scheme;
    using Ibis;

    /// <summary>
    /// A generator of 1B1S file data for an invoice run.
    /// </summary>
    public interface IIbisFileDataGenerator
    {
        /// <summary>
        /// Creates the data representing 1B1S customer and transaction files for the specified list
        /// of member uploads.
        /// </summary>
        /// <param name="fileID">The ID that the 1B1S files will use. This must be unique for every pair of 1B1S files
        /// and must be in the range of 0 to 99999. To avoid clashes with IDs used by the incumbent system, a seed
        /// value may need to be used.</param>
        /// <param name="memberUploads">The list of member uploads to be included.</param>
        /// <returns>Returns an <see cref="IbisFileData"/> which provides the data and file names of the
        /// generated 1B1S customer and transaction files.</returns>
        IbisFileData CreateFileData(ulong fileID, IReadOnlyList<MemberUpload> memberUploads);
    }
}
