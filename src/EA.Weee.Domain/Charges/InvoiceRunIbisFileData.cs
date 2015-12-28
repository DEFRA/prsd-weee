namespace EA.Weee.Domain.Charges
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Prsd.Core;

    /// <summary>
    /// Provides the 1B1S file data for an invoice run.
    /// This object should be accessed through the lazy-loaded IbisFileData property
    /// of an <see cref="InvoiceRun"/> domain obejct.
    /// </summary>
    public class InvoiceRunIbisFileData
    {
        [Key]
        public Guid InvoiceRunId { get; private set; }

        public ulong FileId { get; private set; }

        public string CustomerFileName { get; private set; }

        public string CustomerFileData { get; private set; }

        public string TransactionFileName { get; private set; }

        public string TransactionFileData { get; private set; }

        public InvoiceRunIbisFileData(
            ulong fileId,
            string customerFileName,
            string customerFileData,
            string transactionFileName,
            string transactionFileData)
        {
            Guard.ArgumentNotNullOrEmpty(() => customerFileName, customerFileName);
            Guard.ArgumentNotNullOrEmpty(() => customerFileData, customerFileData);
            Guard.ArgumentNotNullOrEmpty(() => transactionFileName, transactionFileName);
            Guard.ArgumentNotNullOrEmpty(() => transactionFileData, transactionFileData);

            FileId = fileId;
            CustomerFileName = customerFileName;
            CustomerFileData = customerFileData;
            TransactionFileName = transactionFileName;
            TransactionFileData = transactionFileData;
        }

        /// <summary>
        /// This constructor is used by Entity Framework.
        /// </summary>
        protected InvoiceRunIbisFileData()
        {
        }

        /// <summary>
        /// This property is used by Entity Framework to map the ulong FileID property
        /// to the BIGINT column of the database.
        /// </summary>
        public long FileIdDatabaseValue
        {
            get { return (long)FileId; }
            set { FileId = (ulong)value; }
        }
    }
}
