namespace EA.Weee.RequestHandlers.Charges.IssuePendingCharges
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Weee.Domain.Charges;
    using EA.Weee.Ibis;
    using Errors;

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
        private readonly IIbisFileDataErrorTranslator ibisFileDataErrorTranslator;

        public IbisFileDataGenerator(
            IIbisCustomerFileGenerator ibisCustomerFileGenerator,
            IIbisTransactionFileGenerator ibisTransactionFileGenerator,
            IIbisFileDataErrorTranslator ibisFileDataErrorTranslator)
        {
            this.ibisCustomerFileGenerator = ibisCustomerFileGenerator;
            this.ibisTransactionFileGenerator = ibisTransactionFileGenerator;
            this.ibisFileDataErrorTranslator = ibisFileDataErrorTranslator;
        }

        /// <summary>
        /// Creates the data representing 1B1S customer and transaction files for the specified list
        /// of member uploads.
        /// </summary>
        /// <param name="fileID">The ID that the 1B1S files will use. This must be unique for every pair of 1B1S files
        /// and must be in the range of 0 to 99999. To avoid clashes with IDs used by the incumbent system, a seed
        /// value may need to be used.</param>
        /// <param name="invoiceRun">The invoice run specifying the list of member uploads to be included.</param>
        /// <returns>Returns an <see cref="IbisFileDataGeneratorResult"/> which provides the data and file names of the
        /// generated 1B1S customer and transaction files or a list of error which occurred during the process.</returns>
        public async Task<IbisFileDataGeneratorResult> CreateFileDataAsync(ulong fileID, InvoiceRun invoiceRun)
        {
            var customerFileGeneratorResult = await ibisCustomerFileGenerator.CreateAsync(fileID, invoiceRun);
            var ibisTransactionFileGeneratorResult = await ibisTransactionFileGenerator.CreateAsync(fileID, invoiceRun);

            IbisFileData ibisFileData = null;
            var errors = new List<string>();

            if (customerFileGeneratorResult.Errors.Count == 0 &&
                ibisTransactionFileGeneratorResult.Errors.Count == 0)
            {
                CustomerFile customerFile = customerFileGeneratorResult.IbisFile;
                TransactionFile transactionFile = ibisTransactionFileGeneratorResult.IbisFile;

                string customerFileName = string.Format("WEEHC{0:D5}.dat", fileID);
                string transactionFileName = string.Format("WEEHI{0:D5}.dat", fileID);

                string customerFileData = customerFile.Write();
                string transactionFileData = transactionFile.Write();

                ibisFileData = new IbisFileData(
                    fileID,
                    customerFileName,
                    customerFileData,
                    transactionFileName,
                    transactionFileData);
            }
            else
            {
                errors = ibisFileDataErrorTranslator
                    .MakeFriendlyErrorMessages(customerFileGeneratorResult
                                               .Errors
                                               .Union(ibisTransactionFileGeneratorResult.Errors)
                                               .ToList());
            }

            return new IbisFileDataGeneratorResult(ibisFileData, errors);
        }
    }
}
