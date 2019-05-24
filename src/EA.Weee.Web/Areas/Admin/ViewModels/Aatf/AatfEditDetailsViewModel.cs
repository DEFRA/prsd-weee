namespace EA.Weee.Web.Areas.Admin.ViewModels.Aatf
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataStandards;
    using EA.Weee.Core.Shared;

    public class AatfEditDetailsViewModel
    {
        public AatfEditDetailsViewModel()
        {
        }

        public Guid Id { get; set; }

        [Required]
        [StringLength(EnvironmentAgencyMaxFieldLengths.SchemeName)]
        [Display(Name = "Name of Aatf")]
        public string Name { get; set; }

        [Required]
        [RegularExpression(@"WEE/([A-Z]{2}[0-9]{4}[A-Z]{2})/ATF", ErrorMessage = "Approval number is not in correct format")]
        [StringLength(EnvironmentAgencyMaxFieldLengths.SchemeApprovalNumber)]
        [DataType(DataType.Text)]
        [Display(Name = "Approval number")]
        public string ApprovalNumber { get; set; }

        [Required]
        [Display(Name = "Appropriate authority")]
        public Guid CompetentAuthorityId { get; set; }

        public IEnumerable<UKCompetentAuthorityData> CompetentAuthoritiesList { get; set; }

        [Required]
        [Display(Name = "Status")]
        public int AatfStatus { get; set; }

        public IEnumerable<AatfStatus> AatfStatusList { get; set; }

        public AatfAddressData SiteAddress { get; set; }

        public int Size { get; set; }

        public IEnumerable<AatfSize> SizeList { get; set; }

        [Display(Name = "Approval date")]
        [DataType(DataType.Date)]
        public DateTime? ApprovalDate { get; set; }
    }
}