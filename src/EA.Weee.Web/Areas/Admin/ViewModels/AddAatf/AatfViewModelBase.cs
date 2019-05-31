namespace EA.Weee.Web.Areas.Admin.ViewModels.AddAatf
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataStandards;
    using EA.Weee.Core.Shared;

    public abstract class AatfViewModelBase
    {
        [Required]
        [RegularExpression(@"WEE/([A-Z]{2}[0-9]{4}[A-Z]{2})/ATF", ErrorMessage = "Approval number is not in correct format")]
        [StringLength(EnvironmentAgencyMaxFieldLengths.SchemeApprovalNumber)]
        [DataType(DataType.Text)]
        [Display(Name = "Approval number")]
        public string ApprovalNumber { get; set; }

        public IEnumerable<UKCompetentAuthorityData> CompetentAuthoritiesList { get; set; }

        [Required]
        [Display(Name = "Appropriate authority")]
        public Guid CompetentAuthorityId { get; set; }

        [Display(Name = "Approval date")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime? ApprovalDate { get; set; }

        [Required]
        [Display(Name = "Compliance year")]
        public Int16 ComplianceYear { get; set; }

        public IEnumerable<AatfStatus> StatusList { get; set; }

        [Required]
        [Display(Name = "Status")]
        public int StatusValue { get; set; }

        public IEnumerable<AatfSize> SizeList { get; set; }

        [Required]
        [Display(Name = "Size")]
        public int SizeValue { get; set; }
    }
}