namespace EA.Weee.Requests.Charges
{
    using System;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Shared;

    /// <summary>
    /// A request to issue all pending charges for the specified appropriate authority.
    /// If successful, the request returns the ID of the newly created invoice run.
    /// </summary>
    public class IssuePendingCharges : IRequest<Guid>
    {
        public CompetentAuthority Authority { get; private set; }

        public IssuePendingCharges(CompetentAuthority authority)
        {
            Authority = authority;
        }
    }
}
