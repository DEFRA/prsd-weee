namespace EA.Weee.Requests.AatfReturn.Obligated
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using System;
    using System.Collections.Generic;

    public class GetWeeeSentOn : IRequest<List<WeeeSentOnData>>
    {
        public Guid AatfId { get; set; }

        public Guid ReturnId { get; set; }

        public Guid? WeeeSentOnId { get; set; }

        public GetWeeeSentOn(Guid aatfId, Guid returnId, Guid? weeeSentOnId)
        {
            AatfId = aatfId;
            ReturnId = returnId;
            WeeeSentOnId = weeeSentOnId;
        }
    }
}
