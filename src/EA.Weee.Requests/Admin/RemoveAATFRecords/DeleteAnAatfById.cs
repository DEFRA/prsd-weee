namespace EA.Weee.Requests.Admin.RemoveAATFRecords
{
    using EA.Prsd.Core.Mediator;
    using System;

    public class DeleteAnAatfById : IRequest<bool>
    {
        public Guid AatfId { get; private set; }

        public DeleteAnAatfById(Guid aatfId)
        {
            AatfId = aatfId;
        }
    }
}
