namespace EA.Weee.Requests.AatfReturn
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using System;
    using System.Collections.Generic;

    public class GetPreviousQuarterSchemes : IRequest<PreviousQuarterReturnResult>
    {
        public Guid OrganisationId { get; private set; }
        public Guid ReturnId { get; private set; }

        public GetPreviousQuarterSchemes(Guid organisationId, Guid returnId)
        {
            this.OrganisationId = organisationId;
            this.ReturnId = returnId;
        }
    }
}
