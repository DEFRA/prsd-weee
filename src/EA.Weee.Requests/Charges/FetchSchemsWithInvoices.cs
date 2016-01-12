namespace EA.Weee.Requests.Charges
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Shared;

    /// <summary>
    /// This request returns the names of each scheme for the specified authority
    /// that has a member upload which has been invoiced.
    /// Scheme names are returned in alphabetical order.
    /// </summary>
    public class FetchSchemesWithInvoices : IRequest<IReadOnlyList<string>>
    {
        public CompetentAuthority Authority { get; private set; }

        public FetchSchemesWithInvoices(CompetentAuthority authority)
        {
            Authority = authority;
        }
    }
}
