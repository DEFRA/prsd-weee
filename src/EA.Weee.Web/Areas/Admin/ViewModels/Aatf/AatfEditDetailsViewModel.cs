namespace EA.Weee.Web.Areas.Admin.ViewModels.Aatf
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Shared;

    public class AatfEditDetailsViewModel
    {
        public const int MaxLengthName = 50;
        public const int MaxLengthApprovalNumber = 10;

        public AatfEditDetailsViewModel()
        {
        }

        public Guid Id { get; set; }

        [Required]
        [StringLength(MaxLengthName)]
        public string Name { get; set; }

        [Required]
        [StringLength(MaxLengthApprovalNumber)]
        [Display(Name = "Approval number")]
        public string ApprovalNumber { get; set; }

        [Display(Name = "Appropiate authority")]
        public CompetentAuthority CompetentAuthority { get; set; }

        [Display(Name = "Status")]
        public AatfStatusEnum AatfStatus { get; set; }

        public AatfAddressData SiteAddress { get; set; }

        public AatfSizeEnum Size { get; set; }

        [Display(Name = "Approval date")]
        public DateTime? ApprovalDate { get; set; }
    }
}