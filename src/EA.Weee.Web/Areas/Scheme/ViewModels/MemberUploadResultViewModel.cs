namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Core.Shared;
    using Prsd.Core.Validation;

    public class MemberUploadResultViewModel
    {
        public List<MemberUploadErrorData> ErrorData { get; set; }

        public Guid MemberUploadId { get; set; }

        public decimal TotalCharges { get; set; }

        [MustBeTrue(ErrorMessage = "Please confirm that you have read the privacy policy")]
        public bool PrivacyPolicy { get; set; }
    }
}