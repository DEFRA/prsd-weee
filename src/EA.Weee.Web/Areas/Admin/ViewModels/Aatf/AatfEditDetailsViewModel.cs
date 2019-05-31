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
            this.SiteAddress = new AatfAddressData();
        }

        public Guid Id { get; set; }

        private string aatfName;
        [Required]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        [Display(Name = "Name of AATF")]
        public string Name
        {
            get => this.aatfName;

            set
            {
                this.aatfName = value;

                if (this.SiteAddress != null)
                {
                    this.SiteAddress.Name = value;
                }
            }
        }

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

        [Required(ErrorMessage = "Enter size")]
        public int Size { get; set; }

        public IEnumerable<AatfSize> SizeList { get; set; }

        [Display(Name = "Approval date")]
        [DataType(DataType.Date)]
        public DateTime? ApprovalDate { get; set; }

        public FacilityType FacilityType { get; set; }

        [Display(Name = "Compliance year")]
        public Int16 ComplianceYear { get; set; }
    }
}