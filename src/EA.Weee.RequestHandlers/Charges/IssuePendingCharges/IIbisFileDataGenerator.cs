namespace EA.Weee.RequestHandlers.Charges.IssuePendingCharges
{
    using System.Threading.Tasks;
    using Domain.Charges;

    /// <summary>
    /// A generator of 1B1S file data for an invoice run.
    /// </summary>
    public interface IIbisFileDataGenerator
    {
        /// <summary>
        /// Creates the data representing 1B1S customer and transaction files for the specified invoice run.
        /// </summary>
        /// <param name="fileID">The ID that the 1B1S files will use. This must be unique for every pair of 1B1S files
        /// and must be in the range of 0 to 99999. To avoid clashes with IDs used by the incumbent system, a seed
        /// value may need to be used.</param>
        /// <param name="invoiceRun">The invoice run specifying the list of member uploads to be included.</param>
        /// <returns>Returns an <see cref="IbisFileData"/> which provides the data and file names of the
        /// generated 1B1S customer and transaction files.</returns>
        Task<IbisFileData> CreateFileDataAsync(ulong fileID, InvoiceRun invoiceRun);
    }
}
