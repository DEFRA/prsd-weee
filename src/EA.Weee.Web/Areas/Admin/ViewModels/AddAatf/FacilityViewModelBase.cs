namespace EA.Weee.Web.Areas.Admin.ViewModels.AddAatf
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataStandards;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Admin.ViewModels.Validation;
    using FluentValidation.Attributes;

    [Validator(typeof(ApprovalDateValidator))]
    public abstract class FacilityViewModelBase
    {
        [Required]
        [StringLength(EnvironmentAgencyMaxFieldLengths.SchemeApprovalNumber)]
        [DataType(DataType.Text)]
        [Display(Name = "Approval number")]
        public abstract string ApprovalNumber { get; set; }

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
        public short ComplianceYear { get; set; }

        public IEnumerable<AatfStatus> StatusList { get; set; }

        [Required]
        [Display(Name = "Status")]
        public int StatusValue { get; set; }

        public IEnumerable<AatfSize> SizeList { get; set; }

        public FacilityType FacilityType { get; set; }

        [Required]
        [Display(Name = "Size")]
        public int SizeValue { get; set; }
    }
}