namespace EA.Weee.Requests.AatfReturn
{
    using System.Collections.Generic;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;

    public class GetReportOnQuestion : IRequest<List<ReportOnQuestion>>
    {
        public GetReportOnQuestion()
        {
        }
    }
}
