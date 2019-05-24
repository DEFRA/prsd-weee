namespace EA.Weee.Web.Areas.Admin.ViewModels.AddAatf
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataStandards;
    using EA.Weee.Core.Shared;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class AddAatfViewModel
    {
        public Guid OrganisationId { get; set; }
        public string OrganisationName { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        [Display(Name = "Name of AATF")]
        public string AatfName { get; set; }

        public AatfAddressData SiteAddressData { get; set; }

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

        public IEnumerable<AatfStatus> StatusList { get; set; }

        [Required]
        [Display(Name = "Status")]
        public int SelectedStatusValue { get; set; }

        public IEnumerable<AatfSize> SizeList { get; set; }

        [Required]
        [Display(Name = "Size")]
        public int SelectedSizeValue { get; set; }

        [Display(Name = "Approval date")]
        [DataType(DataType.Date)]
        public DateTime? ApprovalDate { get; set; }

        public AatfContactData ContactData { get; set; }

        public AddAatfViewModel()
        {
            this.ContactData = new AatfContactData();
            this.SiteAddressData = new AatfAddressData();
        }
    }
}