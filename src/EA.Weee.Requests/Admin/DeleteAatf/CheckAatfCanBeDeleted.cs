namespace EA.Weee.Requests.Admin.DeleteAatf
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Admin;
    using System;

    public class CheckAatfCanBeDeleted : IRequest<AatfDeletionData>
    {
        public Guid AatfId { get; private set; }

        public CheckAatfCanBeDeleted(Guid aatfId)
        {
            AatfId = aatfId;
        }
    }
}
