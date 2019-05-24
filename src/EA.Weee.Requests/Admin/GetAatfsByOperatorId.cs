namespace EA.Weee.Requests.Admin
{
    using System;
    using System.Collections.Generic;
    using Core.AatfReturn;
    using Prsd.Core.Mediator;

    public class GetAatfsByOperatorId : IRequest<List<AatfDataList>>
    {
        public Guid OperatorId { get; set; }

        public GetAatfsByOperatorId(Guid id)
        {
            OperatorId = id;
        }
    }
}
