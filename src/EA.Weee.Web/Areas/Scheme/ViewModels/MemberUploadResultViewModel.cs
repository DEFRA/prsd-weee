namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Core.Shared;
    using Prsd.Core.Validation;

    public class MemberUploadResultViewModel
    {
        public Guid MemberUploadId { get; set; }

        public Guid PcsId { get; set; }

        public List<ErrorData> ErrorData { get; set; }

        public decimal TotalCharges { get; set; }

        [MustBeTrue(ErrorMessage = "Please confirm that you have read the privacy policy")]
        public bool PrivacyPolicy { get; set; }

        public int? ComplianceYear { get; set; }
    }
}