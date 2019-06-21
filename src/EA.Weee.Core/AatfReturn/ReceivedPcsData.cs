namespace EA.Weee.Core.AatfReturn
{
    using System;
    public class ReceivedPcsData
    {
        public Guid SchemeId { get; set; }

        public string SchemeName { get; set; }

        public string ApprovalNumber { get; set; }

        public ObligatedCategoryValue Tonnages { get; set; }
    }
}
