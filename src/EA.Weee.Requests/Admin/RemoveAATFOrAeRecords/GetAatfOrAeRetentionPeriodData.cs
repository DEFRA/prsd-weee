namespace EA.Weee.Requests.Admin.RemoveAATFOrAeRecords
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Admin;
    using System.Collections.Generic;

    public class GetAatfOrAeRetentionPeriodData : IRequest<List<AatfDataList>>
    {
        public RemoveAatfOrAeFilter Filter { get; private set; }

        public GetAatfOrAeRetentionPeriodData(RemoveAatfOrAeFilter filter = null)
        {
            Filter = filter;
        }
    }
}
