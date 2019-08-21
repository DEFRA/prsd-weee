namespace EA.Weee.Web.ViewModels.Returns
{
    using Core.DataReturns;
    using System;
    using System.Collections.Generic;

    public class ReturnsViewModel
    {
        public Guid OrganisationId { get; set; }

        public string OrganisationName { get; set; }

        public IList<ReturnsItemViewModel> Returns { get; set; }

        public int ComplianceYear { get; set; }

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