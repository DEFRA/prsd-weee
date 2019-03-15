namespace EA.Weee.Core.AatfReturn
{
    using System;
    public class ReceivedPcsData
    {
        public Guid SchemeId { get; set; }

        public string SchemeName { get; set; }

        public string ApprovalNumber { get; set; }

        public string B2bTotal { get; set; }

        public string B2cTotal { get; set; }
    }
}
