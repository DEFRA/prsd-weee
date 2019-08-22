namespace EA.Weee.Requests.Admin.Aatf
{
    using Core.Admin.AatfReports;
    using Prsd.Core.Mediator;
    using System;
    using System.Collections.Generic;

    public class GetAatfSubmissionHistory : IRequest<List<AatfSubmissionHistoryData>>
    {
        public Guid AatfId { get; private set; }

        public GetAatfSubmissionHistory(Guid aatfId)
        {
            AatfId = aatfId;
        }
    }
}
