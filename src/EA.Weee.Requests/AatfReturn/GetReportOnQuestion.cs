namespace EA.Weee.Requests.AatfReturn
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using System.Collections.Generic;

    public class GetReportOnQuestion : IRequest<List<ReportOnQuestion>>
    {
        public GetReportOnQuestion()
        {
        }
    }
}
