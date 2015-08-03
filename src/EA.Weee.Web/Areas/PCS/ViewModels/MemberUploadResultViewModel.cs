namespace EA.Weee.Web.Areas.PCS.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Core.Shared;

    public class MemberUploadResultViewModel
    {
        public List<MemberUploadErrorData> ErrorData { get; set; }

        public Guid MemberUploadId { get; set; }

        public decimal TotalCharges { get; set; }
    }
}