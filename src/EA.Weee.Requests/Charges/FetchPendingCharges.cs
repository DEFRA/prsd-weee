namespace EA.Weee.Requests.Charges
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core.Charges;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Shared;

    /// <summary>
    /// A request to fetch all pending charges for the specified appropriate authority.
    /// Charges are ordered by scheme name and then compliance year descending.
    /// </summary>
    public class FetchPendingCharges : IRequest<IList<PendingCharge>>
    {
        public CompetentAuthority Authority { get; private set; }

        public FetchPendingCharges(CompetentAuthority authority)
        {
            Authority = authority;
        }
    }
}
