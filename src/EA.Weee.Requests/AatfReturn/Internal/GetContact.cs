namespace EA.Weee.Requests.AatfReturn.Internal
{
    using System;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;

    public class GetContact : IRequest<AatfContactData>
    {
        public Guid AatfId { get; private set; }

        public GetContact(Guid aatfId)
        {
            this.AatfId = aatfId;
        }
    }
}
