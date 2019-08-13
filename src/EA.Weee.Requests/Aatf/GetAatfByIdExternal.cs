namespace EA.Weee.Requests.Aatf
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Aatf;
    using System;

    public class GetAatfByIdExternal : IRequest<AatfDataExternal>
    {
        public Guid AatfId { get; set; }

        public GetAatfByIdExternal(Guid id)
        {
            AatfId = id;
        }
    }
}
