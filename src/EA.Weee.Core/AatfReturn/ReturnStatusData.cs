namespace EA.Weee.Core.AatfReturn
{
    using DataReturns;
    using EA.Weee.Core.Scheme;
    using System;
    using System.Collections.Generic;

    public class ReturnStatusData
    {
        public Guid Id { get; set; }

        public Core.AatfReturn.ReturnStatus ReturnStatus { get; set; }

        public bool OtherInProgressReturn { get; set; }

        public Guid OrganisationId { get; set; }

        public ReturnStatusData()
        {
        }
    }
}
