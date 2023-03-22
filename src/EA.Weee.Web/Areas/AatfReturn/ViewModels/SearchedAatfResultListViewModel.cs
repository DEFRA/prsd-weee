namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using EA.Weee.Core.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class SearchedAatfResultListViewModel
    {
        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }

        public Guid AatfId { get; set; }

        public string AatfName { get; set; }

        public List<WeeeSearchedAnAatfListData> Sites { get; set; }

        public Guid SelectedAatfId { get; set; }

        public string SelectedAatfName { get; set; }

        [Required(ErrorMessage = "Select an AATF")]
        public string SelectedSiteName { get; set; }
    }
}