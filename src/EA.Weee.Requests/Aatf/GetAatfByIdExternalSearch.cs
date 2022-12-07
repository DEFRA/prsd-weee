namespace EA.Weee.Requests.Aatf
{
    using EA.Prsd.Core.Mediator;
    using System;
    using EA.Weee.Core.AatfReturn;

    public class GetAatfByIdExternalSearch : IRequest<AatfData>
    {
        public Guid AatfId { get; set; }

        public GetAatfByIdExternalSearch(Guid aatfId)
        {
            AatfId = aatfId;
        }
    }
}
