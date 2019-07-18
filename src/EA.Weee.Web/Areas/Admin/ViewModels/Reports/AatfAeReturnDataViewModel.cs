namespace EA.Weee.Web.Areas.Admin.ViewModels.Reports
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Core.Shared;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Admin;

    public class AatfAeReturnDataViewModel
    {
        [Required]
        [DisplayName("Compliance year")]
        public int SelectedYear { get; set; }

        public IEnumerable<SelectListItem> ComplianceYears { get; set; }

        [Required]
        [DisplayName("Quarter")]
        public int Quarter { get; set; }

        public IEnumerable<SelectListItem> Quarters
        {
            get
            {
                yield return new SelectListItem() { Text = "1" };
                yield return new SelectListItem() { Text = "2" };
                yield return new SelectListItem() { Text = "3" };
                yield return new SelectListItem() { Text = "4" };
            }
        }

        [Required(ErrorMessage = "Enter AATF or AE")]
        [Display(Name = "AATF or AE")]
        public string SelectedFacilityType { get; set; }

        public IEnumerable<SelectListItem> FacilityTypes { get; set; }

        [DisplayName("Submission status")]
        public int? SelectedSubmissionStatus { get; set; }

        public IEnumerable<SelectListItem> SubmissionStatus
        {
            get
            {
                yield return new SelectListItem() { Text = "Submitted", Value = "2" };
                yield return new SelectListItem() { Text = "Started", Value = "1" };
                yield return new SelectListItem() { Text = "Not Started", Value = "0" };
            }
        }

        [Display(Name = "Appropriate authority")]
        public Guid? CompetentAuthorityId { get; set; }

        public IEnumerable<SelectListItem> CompetentAuthoritiesList { get; set; }

        [Display(Name = "WROS PAT")]
        public Guid? PanAreaId { get; set; }
        public IEnumerable<SelectListItem> PanAreaList { get; set; }

        [Display(Name = "EA area")]
        public Guid? LocalAreaId { get; set; }

        public IEnumerable<SelectListItem> LocalAreaList { get; set; }
    }
}