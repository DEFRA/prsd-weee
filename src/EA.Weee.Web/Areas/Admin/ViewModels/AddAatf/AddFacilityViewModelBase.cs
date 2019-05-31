namespace EA.Weee.Web.Areas.Admin.ViewModels.AddAatf
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataStandards;
    using EA.Weee.Core.Shared;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public abstract class AddFacilityViewModelBase
    {
        public Guid OrganisationId { get; set; }

        public string OrganisationName { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        public abstract string Name { get; set; }

        public AatfAddressData SiteAddressData { get; set; }

        [Required]
        [StringLength(EnvironmentAgencyMaxFieldLengths.SchemeApprovalNumber)]
        [DataType(DataType.Text)]
        [Display(Name = "Approval number")]
        public abstract string ApprovalNumber { get; set; }

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

        public IEnumerable<short> ComplianceYearList => new List<short> { 2019 };

        [Required]
        [Display(Name = "Compliance year")]
        public short SelectedComplianceYear { get; set; }

        public AddFacilityViewModelBase()
        {
            ContactData = new AatfContactData();
            SiteAddressData = new AatfAddressData();
        }
    }
}