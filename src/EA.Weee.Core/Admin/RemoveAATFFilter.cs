namespace EA.Weee.Core.Admin
{
    using System;

    public class RemoveAATFFilter
    {
        public RemoveAATFFilter()
        {
        }

        public string Name { get; set; }

        public string ApprovalNumber { get; set; }

        public int? ComplianceYear { get; set; }

        public int? SelectedStatus { get; set; }

        public string UserId { get; set; }
    }
}
