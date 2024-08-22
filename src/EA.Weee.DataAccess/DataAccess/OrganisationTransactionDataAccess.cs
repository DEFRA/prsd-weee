namespace EA.Weee.DataAccess.DataAccess
{
    using EA.Prsd.Core.Domain;
    using EA.Weee.Domain.Organisation;
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    public class OrganisationTransactionDataAccess : IOrganisationTransactionDataAccess
    {
        private readonly WeeeContext context;
        private readonly IUserContext userContext;

        public OrganisationTransactionDataAccess(WeeeContext context, IUserContext userContext)
        {
            this.context = context;
            this.userContext = userContext;
        }

        public async Task<OrganisationTransaction> FindIncompleteTransactionForCurrentUserAsync()
        {
            return await context.OrganisationTransactions
                .Where(ot => ot.UserId == userContext.UserId.ToString())
                .Where(ot => ot.CompletionStatus.Value == CompletionStatus.Incomplete.Value)
                .FirstOrDefaultAsync();
        }

        public async Task<OrganisationTransaction> UpdateOrCreateTransactionAsync(string organisationJson)
        {
            var transaction = await FindIncompleteTransactionForCurrentUserAsync();

            if (transaction == null)
            {
                transaction = new OrganisationTransaction(userContext.UserId.ToString());
                context.OrganisationTransactions.Add(transaction);
            }

            transaction.SetOrganisationJson(organisationJson);

            await context.SaveChangesAsync();

            return transaction;
        }

        public async Task<OrganisationTransaction> CompleteTransactionAsync(string organisationJson)
        {
            var transaction = await FindIncompleteTransactionForCurrentUserAsync();

            if (transaction == null)
            {
                throw new InvalidOperationException("No incomplete transaction found for the current user.");
            }

            transaction.SetOrganisationJson(organisationJson);
            transaction.CompleteRegistration();

            await context.SaveChangesAsync();

            return transaction;
        }
    }
}
