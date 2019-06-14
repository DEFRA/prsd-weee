namespace EA.Weee.Requests.Admin.DeleteAatf
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Admin;
    using System;

    public class CheckAatfCanBeDeleted : IRequest<CanAatfBeDeletedFlags>
    {
        public Guid AatfId { get; set; }
    }
}
