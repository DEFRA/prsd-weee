﻿namespace EA.Weee.RequestHandlers.Charges.IssuePendingCharges
{
    using System.Threading.Tasks;
    using EA.Weee.DataAccess;

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
