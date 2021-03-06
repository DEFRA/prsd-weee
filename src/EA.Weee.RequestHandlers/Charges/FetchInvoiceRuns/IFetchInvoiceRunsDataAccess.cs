﻿namespace EA.Weee.RequestHandlers.Charges.FetchInvoiceRuns
{
    using Domain;
    using Domain.Charges;
    using Shared;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IFetchInvoiceRunsDataAccess : ICommonDataAccess
    {
        /// <summary>
        /// Fetches all invoice runs for the specified authority.
        /// Results are ordered by issued date descending.
        /// </summary>
        /// <param name="authority"></param>
        /// <returns></returns>
        Task<IReadOnlyList<InvoiceRun>> FetchInvoiceRunsAsync(UKCompetentAuthority authority);
    }
}
