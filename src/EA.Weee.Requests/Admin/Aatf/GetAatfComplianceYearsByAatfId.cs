namespace EA.Weee.Requests.Admin.Aatf
{
    using System;
    using System.Collections.Generic;
    using Prsd.Core.Mediator;

    public class GetAatfComplianceYearsByAatfId : IRequest<List<short>>
    {
        public Guid AatfId { get; private set; }

        public GetAatfComplianceYearsByAatfId(Guid aatfId)
        {
            this.AatfId = aatfId;
        }
    }
}
