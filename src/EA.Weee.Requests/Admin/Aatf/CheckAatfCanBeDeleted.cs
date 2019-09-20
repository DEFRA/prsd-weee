namespace EA.Weee.Requests.Admin.Aatf
{
    using Core.Admin;
    using Prsd.Core.Mediator;
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
