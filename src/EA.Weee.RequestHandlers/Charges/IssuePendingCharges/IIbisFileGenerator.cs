namespace EA.Weee.RequestHandlers.Charges.IssuePendingCharges
{
    using Domain.Charges;
    using Ibis;
    using System.Threading.Tasks;

    public interface IIbisFileGenerator<T> where T : IbisFile
    {
        Task<IbisFileGeneratorResult<T>> CreateAsync(ulong fileID, InvoiceRun invoiceRun);
    }
}
