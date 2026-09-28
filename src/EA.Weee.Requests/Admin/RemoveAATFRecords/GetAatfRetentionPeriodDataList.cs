namespace EA.Weee.Requests.Admin.RemoveAATFRecords
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Admin;
    using System.Collections.Generic;

    public class GetAatfRetentionPeriodDataList : IRequest<List<AatfDataList>>
    {
        public RemoveAATFFilter Filter { get; private set; }

        public GetAatfRetentionPeriodDataList(RemoveAATFFilter filter = null)
        {
            Filter = filter;
        }
    }
}
