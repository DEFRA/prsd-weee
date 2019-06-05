namespace EA.Weee.Requests.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using Core.AatfReturn;
    using Prsd.Core.Mediator;

    public class GetReturns : IRequest<ReturnsData>
    {
        public Guid OrganisationId { get; private set; }

        public GetReturns(Guid organisationId)
        {
            this.OrganisationId = organisationId;
        }
    }
}
