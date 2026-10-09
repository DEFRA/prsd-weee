namespace EA.Weee.Requests.Admin.RemoveAATFOrAeRecords
{
    using EA.Prsd.Core.Mediator;
    using System;

    public class DeleteAatfOrAeRecordById : IRequest<bool>
    {
        public Guid AatfId { get; private set; }

        public DeleteAatfOrAeRecordById(Guid aatfId)
        {
            AatfId = aatfId;
        }
    }
}
