namespace EA.Weee.Requests.Admin.Aatf
{
    using System;
    using Core.AatfReturn;
    using Prsd.Core.Mediator;

    public class GetAatfContact : IRequest<AatfContactData>
    {
        public Guid AatfId { get; private set; }

        public GetAatfContact(Guid aatfId)
        {
            this.AatfId = aatfId;
        }
    }
}
