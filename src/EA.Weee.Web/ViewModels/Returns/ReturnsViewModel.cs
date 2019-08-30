namespace EA.Weee.Web.ViewModels.Returns
{
    using Core.DataReturns;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public class ReturnsViewModel
    {
        public Guid OrganisationId { get; set; }

        public string OrganisationName { get; set; }

        public IList<ReturnsItemViewModel> Returns { get; set; }

        public int ComplianceYear { get; set; }

        public List<int> ComplianceYearList { get; set; }

        [DisplayName("Compliance year")]
        public int SelectedComplianceYear { get; set; }

        public List<string> QuarterList { get; set; }

        [DisplayName("Reporting quarter")]
        public string SelectedQuarter { get; set; }

        public QuarterType Quarter { get; set; }

        public bool DisplayCreateReturn { get; set; }

        public ReturnsViewModel()
        {
            Returns = new List<ReturnsItemViewModel>();
        }

        public string ErrorMessageForNotAllowingCreateReturn { get; set; }

        public bool NotStartedAnySubmissionsYet { get; set; }
    }
}