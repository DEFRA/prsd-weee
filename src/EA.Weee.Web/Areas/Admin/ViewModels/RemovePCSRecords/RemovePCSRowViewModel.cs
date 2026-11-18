namespace EA.Weee.Web.Areas.Admin.ViewModels.RemovePCSRecords
{
    using System;

    [Serializable]
    public class RemovePCSRowViewModel
    {
        public Guid SchemeId { get; set; }

        public string SchemeName { get; set; }

        public string ApprovalNumber { get; set; }

        public int ComplianceYear { get; set; }
    }
}