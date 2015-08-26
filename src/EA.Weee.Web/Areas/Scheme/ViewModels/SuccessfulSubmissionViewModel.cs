namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System;

    public class SuccessfulSubmissionViewModel
    {
        public Guid PcsId { get; set; }

        public Guid MemberUploadId { get; set; }

        public int ComplianceYear { get; set; }
    }
}