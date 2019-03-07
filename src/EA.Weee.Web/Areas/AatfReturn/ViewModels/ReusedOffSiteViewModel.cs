namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Web.ViewModels.Shared;

    public class ReusedOffSiteViewModel : RadioButtonStringCollectionViewModel
    {
        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }

        public Guid AatfId { get; set; }

        public String AatfName { get; set; }

        [Required(ErrorMessage = "You must tell us whether any of this WEEE has gone directly to another site to be reused")]
        public override string SelectedValue { get; set; }

        public ReusedOffSiteViewModel(Guid organisationId, Guid returnId, Guid aatfId, string aatfName, string selectedValue) : base(new List<string> { "Yes", "No" })
        {
            OrganisationId = organisationId;
            ReturnId = returnId;
            AatfId = aatfId;
            AatfName = aatfName;
            SelectedValue = selectedValue;
        }
        public ReusedOffSiteViewModel() : base(new List<string> { "Yes", "No" })
        {
        }
    }
}