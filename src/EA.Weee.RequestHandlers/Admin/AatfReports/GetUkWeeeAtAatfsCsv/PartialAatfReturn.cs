namespace EA.Weee.RequestHandlers.Admin.AatfReports.GetUkWeeeAtAatfsCsv
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.DataReturns;
    using System.Collections.Generic;

    public class PartialAatfReturn
    {
        public Quarter Quarter { get; set; }

        public IList<WeeeObligatedData> ObligatedWeeeReceivedData { get; set; }

        public IList<WeeeObligatedData> ObligatedWeeeReusedData { get; set; }

        public IList<WeeeObligatedData> ObligatedWeeeSentOnData { get; set; }
    }
}