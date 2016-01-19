namespace EA.Weee.RequestHandlers.Charges.IssuePendingCharges
{
    using System.Threading.Tasks;
    using Domain.Charges;
    using Ibis;

    public interface IIbisFileGenerator<T> where T : IbisFile
    {
        Task<IbisFileGeneratorResult<T>> CreateAsync(ulong fileID, InvoiceRun invoiceRun);
    }
}
