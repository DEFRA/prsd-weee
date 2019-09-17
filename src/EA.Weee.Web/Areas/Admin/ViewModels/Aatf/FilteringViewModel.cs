namespace EA.Weee.Web.Areas.Admin.ViewModels.Aatf
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Shared;

    public class FilteringViewModel
    {
        public string Name { get; set; }

        [Display(Name = "Approval number")]
        public string ApprovalNumber { get; set; }

        public FacilityType FacilityType { get; set; }

        public List<int> SelectedReturnStatus { get; set; }

        public List<Guid> SelectedAuthority { get; set; }

        public List<UKCompetentAuthorityData> CompetentAuthorityOptions { get; set; }
    }
}