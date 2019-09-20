namespace EA.Weee.Requests.AatfReturn
{
    using Core.AatfReturn;
    using Prsd.Core.Mediator;
    using System;

    public class GetAatfById : IRequest<AatfData>
    {
        public Guid AatfId { get; set; }

        public GetAatfById(Guid id)
        {
            AatfId = id;
        }
    }
}
