namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Core.Scheme.MemberUploadTesting;
    using Core.Shared;
    using Prsd.Core.Helpers;

    public class SchemeViewModel
    {
        public SchemeViewModel()
        {
            ObligationTypeSelectList = new SelectList(EnumHelper.GetValues(typeof(ObligationType)), "Key", "Value");
        }

        [Required(ErrorMessage = "Approval number is required.")]
        [StringLength(70)]
        [DataType(DataType.Text)]
        [Display(Name = "Approval number")]
        public string ApprovalNumber { get; set; }

        [Required(ErrorMessage = "Scheme name is required.")]
        [RegularExpression(@"WEE/([A-Z]{2}[0-9]{4}[A-Z]{2})/SCH?",
            ErrorMessage = "Scheme name is not in correct format.")]
        [DataType(DataType.Text)]
        [Display(Name = "Scheme name")]
        public string SchemeName { get; set; }

        [StringLength(10)]
        [DataType(DataType.Text)]
        [Display(Name = "IBIS customer reference")]
        public string IbisCustomerReference { get; set; }

        [Required(ErrorMessage = "Obligation type is required.")]
        [Display(Name = "Obligation type")]
        public ObligationType ObligationType { get; set; }

        [Required(ErrorMessage = "Authorising authority is required.")]
        [Display(Name = "Authorising authority")]
        public Guid CompetentAuthorityId { get; set; }

        [Display(Name = "Authorising authority")]
        public string CompetentAuthorityName { get; set; }

        public IEnumerable<UKCompetentAuthorityData> CompetentAuthorities { get; set; }

        public IEnumerable<SelectListItem> ObligationTypeSelectList { get; set; }

        public Guid SchemeId { get; set; }
    }
}