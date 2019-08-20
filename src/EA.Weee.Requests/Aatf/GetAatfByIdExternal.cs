namespace EA.Weee.Requests.Aatf
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Aatf;
    using System;

    public class GetAatfByIdExternal : IRequest<AatfDataExternal>
    {
        public Guid AatfId { get; set; }

        public Guid OrganisationId { get; set; }

        public GetAatfByIdExternal(Guid aatfId, Guid orgId)
        {
            AatfId = aatfId;
            OrganisationId = orgId;
        }
    }
}
