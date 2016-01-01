namespace EA.Weee.RequestHandlers.Charges.IssuePendingCharges
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
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
            int transactionNumber = await context.StoredProcedures.SpgNext1B1STransactionNumber();

            return string.Format("WEE{0:D6}H", transactionNumber);
        }
    }
}
