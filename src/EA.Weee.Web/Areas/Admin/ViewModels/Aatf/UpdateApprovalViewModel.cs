namespace EA.Weee.Web.Areas.Admin.ViewModels.Aatf
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Admin;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Web.ViewModels.Shared;
    using Weee.Requests.Admin.Aatf;

    public class UpdateApprovalViewModel : RadioButtonStringCollectionViewModel
    {
        public Guid AatfId { get; set; }

        public Guid OrganisationId { get; set; }

        public CanApprovalDateBeChangedFlags UpdateApprovalDateData { get; set; }

        public string OrganisationName { get; set; }

        public string AatfName { get; set; }

        public FacilityType FacilityType { get; set; }

        public UpdateAatfDetailsTransferViewModel ExistingViewModel { get; set; }

        public EditAatfDetails Request { get; set; }

        [Required(ErrorMessage = "You must select an option")]
        public override string SelectedValue { get; set; }

        public UpdateApprovalViewModel() : base(new List<string> { "Yes", "No" })
        {
        }
    }
}