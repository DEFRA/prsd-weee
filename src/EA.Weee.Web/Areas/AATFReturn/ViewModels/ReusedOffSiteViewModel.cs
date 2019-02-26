namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Web.ViewModels.Shared;

    public class ReusedOffSiteViewModel : YesNoChoiceViewModel
    {
        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }

        [Required(ErrorMessage = "You must tell us whether any of this WEEE has gone directly to another site to be reused")]
        public override string SelectedValue { get; set; }
    }
}