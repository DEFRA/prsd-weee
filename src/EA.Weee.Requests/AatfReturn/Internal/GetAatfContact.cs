namespace EA.Weee.Requests.AatfReturn.Internal
{
    using System;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;

    public class GetAatfContact : IRequest<AatfContactData>
    {
        public Guid AatfId { get; private set; }

        public GetAatfContact(Guid aatfId)
        {
            this.AatfId = aatfId;
        }
    }
}
