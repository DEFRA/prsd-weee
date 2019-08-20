namespace EA.Weee.Requests.Admin.Aatf
{
    using System;
    using Core.Admin;
    using Prsd.Core.Mediator;

    public class CheckAatfCanBeDeleted : IRequest<AatfDeletionData>
    {
        public Guid AatfId { get; private set; }

        public CheckAatfCanBeDeleted(Guid aatfId)
        {
            AatfId = aatfId;
        }
    }
}
