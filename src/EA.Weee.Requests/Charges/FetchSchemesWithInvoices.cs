﻿namespace EA.Weee.Requests.Charges
{
    using Core.Scheme;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Shared;
    using System.Collections.Generic;

    /// <summary>
    /// This request returns the names of each scheme for the specified authority
    /// that has a member upload which has been invoiced.
    /// Scheme names are returned in alphabetical order.
    /// </summary>
    public class FetchSchemesWithInvoices : IRequest<IReadOnlyList<SchemeData>>
    {
        public CompetentAuthority Authority { get; private set; }

        public FetchSchemesWithInvoices(CompetentAuthority authority)
        {
            Authority = authority;
        }
    }
}
