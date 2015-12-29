namespace EA.Weee.RequestHandlers.Charges.IssuePendingCharges
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Weee.Domain.Charges;
    using EA.Weee.Domain.Scheme;
    using EA.Weee.Ibis;

    /// <summary>
    /// This is the default implementation of <see cref="IIbisFileDataGenerator"/>.
    /// It creates an <see cref="IbisFileData"/> using an
    /// <see cref="IIbisCustomerFileGenerator"/> and an <see cref="IIbisTransactionFileGenerator"/>
    /// to generate 1B1S files, which are then written to strings and returned with appropriate
    /// files names based on the specified file ID.
    /// </summary>
    public class IbisFileDataGenerator : IIbisFileDataGenerator
    {
        private readonly IIbisCustomerFileGenerator ibisCustomerFileGenerator;
        private readonly IIbisTransactionFileGenerator ibisTransactionFileGenerator;

        public IbisFileDataGenerator(
            IIbisCustomerFileGenerator ibisCustomerFileGenerator,
            IIbisTransactionFileGenerator ibisTransactionFileGenerator)
        {
            this.ibisCustomerFileGenerator = ibisCustomerFileGenerator;
            this.ibisTransactionFileGenerator = ibisTransactionFileGenerator;
        }

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
        public async Task<IbisFileData> CreateFileDataAsync(ulong fileID, IReadOnlyList<MemberUpload> memberUploads)
        {
            CustomerFile customerFile = await ibisCustomerFileGenerator.CreateAsync(fileID, memberUploads);
            TransactionFile transactionFile = await ibisTransactionFileGenerator.CreateAsync(fileID, memberUploads);

            string customerFileName = string.Format("WEEHC{0:D5}.dat", fileID);
            string transactionFileName = string.Format("WEEHI{0:D5}.dat", fileID);

            string customerFileData = customerFile.Write();
            string transactionFileData = transactionFile.Write();

            return new IbisFileData(
                fileID,
                customerFileName,
                customerFileData,
                transactionFileName,
                transactionFileData);
        }
    }
}
