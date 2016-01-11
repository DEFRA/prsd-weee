namespace EA.Weee.RequestHandlers.Charges.FetchInvoiceRuns
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Domain;
    using Domain.Charges;

    public interface IFetchInvoiceRunsDataAccess : ICommonDataAccess
    {
        /// <summary>
        /// Fetches all invoice runs for the specified authority.
        /// Results are ordered by issued date ascending.
        /// </summary>
        /// <param name="authority"></param>
        /// <returns></returns>
        Task<IReadOnlyList<InvoiceRun>> FetchInvoiceRunsAsync(UKCompetentAuthority authority);
    }
}
