namespace EA.Weee.Web.Areas.Admin.ViewModels.AatfReports
{
    using Core.AatfReturn;
    using Extensions;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class AatfAeReturnDataViewModel
    {
        [Required(ErrorMessage = "Select a compliance year")]
        [DisplayName("Compliance year")]
        public int SelectedYear { get; set; }

        public IEnumerable<SelectListItem> ComplianceYears { get; set; }

        [Required(ErrorMessage = "Select a quarter")]
        [DisplayName("Quarter")]
        public int Quarter { get; set; }

        public IEnumerable<SelectListItem> Quarters
        {
            get
            {
                yield return new SelectListItem() { Text = "Q1", Value = "1" };
                yield return new SelectListItem() { Text = "Q2", Value = "2" };
                yield return new SelectListItem() { Text = "Q3", Value = "3" };
                yield return new SelectListItem() { Text = "Q4", Value = "4" };
            }
        }

        [Required(ErrorMessage = "Select to include or exclude resubmissions")]
        [DisplayName("Include resubmissions?")]
        public bool? IncludeResubmissions { get; set; }

        public IEnumerable<SelectListItem> IncludeResubmissionsOptions
        {
            get
            {
                yield return new SelectListItem() { Text = "Exclude resubmissions", Value = bool.FalseString, Selected = true };
                yield return new SelectListItem() { Text = "Include resubmissions", Value = bool.TrueString };
            }
        }

        [Required(ErrorMessage = "Select AATF or AE")]
        [Display(Name = "AATF or AE")]
        public string SelectedFacilityType { get; set; }

        public IEnumerable<SelectListItem> FacilityTypes { get; set; }

        [DisplayName("Submission status")]
        public int? SelectedSubmissionStatus { get; set; }

        public IEnumerable<SelectListItem> SubmissionStatus
        {
            get
            {
                yield return new SelectListItem() { Text = ReportReturnStatus.Submitted.ToDisplayString<ReportReturnStatus>(), Value = ((int)ReportReturnStatus.Submitted).ToString() };
                yield return new SelectListItem() { Text = ReportReturnStatus.Started.ToDisplayString<ReportReturnStatus>(), Value = ((int)ReportReturnStatus.Started).ToString() };
                yield return new SelectListItem() { Text = ReportReturnStatus.NotStarted.ToDisplayString<ReportReturnStatus>(), Value = ((int)ReportReturnStatus.NotStarted).ToString() };
            }
        }

        [Display(Name = "Appropriate authority")]
        public Guid? CompetentAuthorityId { get; set; }

        public IEnumerable<SelectListItem> CompetentAuthoritiesList { get; set; }

        [Display(Name = "WROS Pan Area Team")]
        public Guid? PanAreaId { get; set; }
        public IEnumerable<SelectListItem> PanAreaList { get; set; }

        [Display(Name = "EA Area")]
        public Guid? LocalAreaId { get; set; }

        public IEnumerable<SelectListItem> LocalAreaList { get; set; }
    }
}