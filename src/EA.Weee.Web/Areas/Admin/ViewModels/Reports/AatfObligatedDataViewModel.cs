namespace EA.Weee.Web.Areas.Admin.ViewModels.Reports
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using EA.Weee.Core.DataStandards;
    using EA.Weee.Core.Shared;

    public class AatfObligatedDataViewModel
    {
        [Required]
        [DisplayName("Compliance year")]
        public int SelectedYear { get; set; }

        public IEnumerable<SelectListItem> ComplianceYears { get; set; }

        [Display(Name = "Appropriate authority")]
        public Guid? CompetentAuthorityId { get; set; }

        public IEnumerable<SelectListItem> CompetentAuthoritiesList { get; set; }

        [Display(Name = "WROS Pan area team")]
        public Guid? PanAreaId { get; set; }
        public IEnumerable<SelectListItem> PanAreaList { get; set; }

        [Required(ErrorMessage = "You must tell us if you want PCS names or Approval numbers as column headings in the report")]
        [DisplayName("Select a column name")]
        public int SelectedColumn { get; set; }

        public IEnumerable<SelectListItem> SchemeColumnPossibleValues
        {
            get
            {
                yield return new SelectListItem() { Text = "PCS names", Value = "1"};
                yield return new SelectListItem() { Text = "Approval numbers", Value = "2" };
            }
        }

        [Display(Name = "Obligation type")]
        public ObligationType? SelectedObligationType { get; set; }

        public IEnumerable<SelectListItem> ObligationTypes
        {
            get
            {
                yield return new SelectListItem() { Text = "B2B" };
                yield return new SelectListItem() { Text = "B2C" };
            }
        }

        [DisplayName("AATF name")]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        public string AATFName { get; set; }
    }
}