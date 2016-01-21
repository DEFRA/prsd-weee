namespace EA.Weee.Requests.Charges
{
    using System.Collections.Generic;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Shared;

    /// <summary>
    /// This request returns each compliance year that has at least one invoiced member upload
    /// for the specified authority.
    /// Years are returned in ascending numerical order.
    /// </summary>
    public class FetchComplianceYearsWithInvoices : IRequest<IReadOnlyList<int>>
    {
        public CompetentAuthority Authority { get; private set; }

        public FetchComplianceYearsWithInvoices(CompetentAuthority authority)
        {
            Authority = authority;
        }
    }
}
