namespace EA.Weee.Web.ViewModels.PCS
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Core.Shared;

    public class MemberUploadResultViewModel
    {
        public List<MemberUploadErrorData> ErrorData { get; set; }

        [Display(Name = "I confirm that the information in the XML file is accurate.")]
        public bool Confirmation { get; set; }
    }
}