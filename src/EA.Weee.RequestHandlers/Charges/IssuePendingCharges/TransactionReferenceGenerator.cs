namespace EA.Weee.RequestHandlers.Charges.IssuePendingCharges
{
    using EA.Weee.DataAccess;
    using System.Threading.Tasks;

    public class TransactionReferenceGenerator : ITransactionReferenceGenerator
    {
        private readonly WeeeContext context;

        public TransactionReferenceGenerator(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<string> GetNextTransactionReferenceAsync()
        {
            int transactionNumber = await context.StoredProcedures.SpgNextIbisTransactionNumber();

            return string.Format("WEE{0:D6}H", transactionNumber);
        }
    }
}
