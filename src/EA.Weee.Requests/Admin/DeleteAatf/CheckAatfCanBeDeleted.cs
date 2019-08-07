namespace EA.Weee.Requests.Admin.DeleteAatf
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Admin;
    using System;

    public class CheckAatfCanBeDeleted : IRequest<AatfDeletionData>
    {
        public Guid AatfId { get; set; }

        public Guid OrganisationId { get; set; }

        public CheckAatfCanBeDeleted(Guid aatfId, Guid organisationId)
        {
            AatfId = aatfId;
            OrganisationId = organisationId;
        }
    }
}
