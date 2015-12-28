namespace EA.Weee.RequestHandlers.Charges.IssuePendingCharges
{
    using System.Collections.Generic;
    using EA.Weee.Domain.Scheme;
    using EA.Weee.Ibis;

    /// <summary>
    /// A generator of 1B1S transaction files based on a list of member uploads.
    /// </summary>
    public interface IIbisTransactionFileGenerator
    {
        TransactionFile CreateTransactionFile(ulong fileID, IReadOnlyList<MemberUpload> memberUploads);
    }
}