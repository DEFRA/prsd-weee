namespace EA.Weee.Web.Areas.Admin.ViewModels.Aatf
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using EA.Prsd.Core.Helpers;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Shared;

    public class FilteringViewModel
    {
        public string Name { get; set; }

        [Display(Name = "Approval number")]
        public string ApprovalNumber { get; set; }

        public FacilityType FacilityType { get; set; }

        public bool SelectApproved { get; set; }

        public bool SelectCancelled { get; set; }

        public bool SelectSuspended { get; set; }

        public List<UKCompetentAuthorityData> CompetentAuthorityOptions { get; set; }

        public List<int> SelectedStatus
        {
            get
            {
                List<int> status = new List<int>();
                if (SelectApproved)
                {
                    status.Add(AatfStatus.Approved.Value);
                }

                if (SelectCancelled)
                {
                    status.Add(AatfStatus.Cancelled.Value);
                }

                if (SelectSuspended)
                {
                    status.Add(AatfStatus.Suspended.Value);
                }
                return status;
            }
        }

        public List<Guid> SelectedAuthority
        {
            get
            {
                return CompetentAuthorityOptions.Where(x => x.Selected).Select(x => x.Id).ToList();
            }
        }
    }
}