namespace EA.Weee.Requests.Admin.Aatf
{
    using System;
    using System.Collections.Generic;
    using Core.AatfReturn;
    using Core.Admin;
    using Core.Admin.AatfReports;
    using Prsd.Core.Mediator;

    public class GetAatfSubmissionHistory : IRequest<List<AatfSubmissionHistoryData>>
    {
        public Guid AatfId { get; private set; }

        public GetAatfSubmissionHistory(Guid aatfId)
        {
            AatfId = aatfId;
        }
    }
}
