namespace EA.Weee.Web.ViewModels.Returns
{
    using Core.DataReturns;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    public class ReturnsViewModel
    {
        public Guid OrganisationId { get; set; }

        public string OrganisationName { get; set; }

        public IList<ReturnsItemViewModel> Returns { get; set; }

        public int ComplianceYear { get; set; }

        public List<int> ComplianceYearList { get; set; }

        [DisplayName("Compliance year")]
        public int SelectedComplianceYear { get; set; }

        private List<string> quarterList;
        public List<string> QuarterList
        {
            get
            {
                if (!this.quarterList.Contains("All"))
                {
                    this.quarterList.Add("All");
                }

                return this.quarterList.OrderBy(p => p).ToList();
            }

            set => this.quarterList = value;
        }

        [DisplayName("Reporting quarter")]
        public string SelectedQuarter { get; set; }

        public QuarterType Quarter { get; set; }

        public bool DisplayCreateReturn { get; set; }

        public ReturnsViewModel()
        {
            Returns = new List<ReturnsItemViewModel>();
        }

        public int NumberOfReturns { get; set; }

        public string ErrorMessageForNotAllowingCreateReturn { get; set; }

        public bool NotStartedAnySubmissionsYet { get; set; }
    }
}