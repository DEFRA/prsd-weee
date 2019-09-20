namespace EA.Weee.Requests.Admin.Aatf
{
    using Core.AatfReturn;
    using Prsd.Core.Mediator;
    using System;

    public class GetAatfContact : IRequest<AatfContactData>
    {
        public Guid AatfId { get; private set; }

        public GetAatfContact(Guid aatfId)
        {
            this.AatfId = aatfId;
        }
    }
}
