namespace EA.Weee.Requests.Charges
{
    using System.Collections.Generic;
    using Core.Shared;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Charges;

    /// <summary>
    /// A request to fetch all invoice runs for the specified appropriate authority.
    /// Invoice runs are ordered by issued date ascending.
    /// </summary>
    public class FetchInvoiceRuns : IRequest<IReadOnlyList<InvoiceRunInfo>>
    {
        public CompetentAuthority Authority { get; private set; }

        public FetchInvoiceRuns(CompetentAuthority authority)
        {
            Authority = authority;
        }
    }
}
