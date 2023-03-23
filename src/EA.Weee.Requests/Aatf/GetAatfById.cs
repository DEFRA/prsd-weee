namespace EA.Weee.Requests.Aatf
{
    using EA.Prsd.Core.Mediator;
    using System;
    using EA.Weee.Core.AatfReturn;

    public class GetAatfById : IRequest<AatfData>
    {
        public Guid AatfId { get; set; }

        public GetAatfById(Guid aatfId)
        {
            AatfId = aatfId;
        }
    }    
}
