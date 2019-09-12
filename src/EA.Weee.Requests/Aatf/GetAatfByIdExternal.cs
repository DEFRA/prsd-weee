namespace EA.Weee.Requests.Aatf
{
    using EA.Prsd.Core.Mediator;

    using System;

    using EA.Weee.Core.AatfReturn;

    public class GetAatfByIdExternal : IRequest<AatfData>
    {
        public Guid AatfId { get; set; }

        public GetAatfByIdExternal(Guid aatfId)
        {
            AatfId = aatfId;
        }
    }
}
