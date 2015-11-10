namespace EA.Weee.Web.Areas.Admin.ViewModels.Scheme
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Core.Shared;
    using Prsd.Core.Helpers;

    public class SchemeViewModel
    {
        public SchemeViewModel()
        {
            ObligationTypeSelectList = new SelectList(EnumHelper.GetValues(typeof(ObligationType)), "Key", "Value");
            StatusSelectList = new SelectList(EnumHelper.GetValues(typeof(SchemeStatus)), "Key", "Value");
            Status = SchemeStatus.Pending;
            IsUnchangeableStatus = false;
        }

        [Required]
        [RegularExpression(@"WEE/([A-Z]{2}[0-9]{4}[A-Z]{2})/SCH",
            ErrorMessage = "Approval number is not in correct format.")]
        [DataType(DataType.Text)]
        [Display(Name = "Approval number")]
        public string ApprovalNumber { get; set; }

        public string OldApprovalNumber { get; set; }

        [Required]
        [StringLength(70)]
        [DataType(DataType.Text)]
        [Display(Name = "Scheme name")]
        public string SchemeName { get; set; }

        [StringLength(10)]
        [DataType(DataType.Text)]
        [Display(Name = "Billing reference")]
        public string IbisCustomerReference { get; set; }

        [Required]
        [Display(Name = "Obligation type")]
        public ObligationType? ObligationType { get; set; }

        [Required]
        [Display(Name = "Appropriate authority")]
        public Guid CompetentAuthorityId { get; set; }

        [Display(Name = "Appropriate authority")]
        public string CompetentAuthorityName { get; set; }

        [Required]
        [Display(Name = "Status")]
        public SchemeStatus Status { get; set; }

        public bool IsUnchangeableStatus { get; set; }

        public Guid SchemeId { get; set; }

        public Guid OrganisationId { get; set; }

        public IEnumerable<UKCompetentAuthorityData> CompetentAuthorities { get; set; }

        public IEnumerable<SelectListItem> ObligationTypeSelectList { get; set; }

        public IEnumerable<SelectListItem> StatusSelectList { get; set; }

        public List<int> ComplianceYears { get; set; }
    }
}