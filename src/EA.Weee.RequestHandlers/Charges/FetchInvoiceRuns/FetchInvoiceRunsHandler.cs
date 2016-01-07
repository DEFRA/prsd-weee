namespace EA.Weee.RequestHandlers.Charges.FetchInvoiceRuns
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Domain;
    using Domain.Charges;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Charges;
    using Security;
    using Requests.Charges;

    public class FetchInvoiceRunsHandler : IRequestHandler<FetchInvoiceRuns, IReadOnlyList<InvoiceRunInfo>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IFetchInvoiceRunsDataAccess dataAccess;

        public FetchInvoiceRunsHandler(
            IWeeeAuthorization authorization,
            IFetchInvoiceRunsDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
        }

        public async Task<IReadOnlyList<InvoiceRunInfo>> HandleAsync(FetchInvoiceRuns message)
        {
            authorization.EnsureCanAccessInternalArea();

            UKCompetentAuthority authority = await dataAccess.FetchCompetentAuthority(message.Authority);

            IReadOnlyList<InvoiceRun> invoiceRuns = await dataAccess.FetchInvoiceRunsAsync(authority);

            List<InvoiceRunInfo> results = new List<InvoiceRunInfo>();

            foreach (InvoiceRun invoiceRun in invoiceRuns)
            {
                results.Add(new InvoiceRunInfo()
                {
                    InvoiceRunId = invoiceRun.Id,
                    IssuedDate = invoiceRun.IssuedDate,
                    IssuedByUserFullName = invoiceRun.IssuedByUser.FullName
                });
            }

            return results;
        }
    }
}
