namespace EA.Weee.Web.Areas.Admin.ViewModels.Aatf
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataStandards;
    using EA.Weee.Core.Shared;

    public abstract class FacilityViewModelBase : IValidatableObject
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        public abstract string Name { get; set; }

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

        public AatfAddressData SiteAddressData { get; set; }

        public FacilityViewModelBase()
        {
            SiteAddressData = new AatfAddressData();
        }

        public bool ModelValidated { get; private set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validationResults = new List<ValidationResult>();
            var instance = validationContext.ObjectInstance as FacilityViewModelBase;

            if (instance?.ApprovalDate != null)
            {
                var input = instance.ApprovalDate.Value;

                var startDate = new DateTime(instance.ComplianceYear, 1, 1);
                var endDate = new DateTime(instance.ComplianceYear, 12, 31);

                if (input < startDate || input > endDate)
                {
                    validationResults.Add(
                        new ValidationResult($"Approval date must be between {startDate.ToString("dd/MM/yyyy")} and {endDate.ToString("dd/MM/yyyy")}",
                        new List<string> { nameof(instance.ApprovalDate) }));
                }
            }

            ModelValidated = true;
            return validationResults;
        }
    }
}