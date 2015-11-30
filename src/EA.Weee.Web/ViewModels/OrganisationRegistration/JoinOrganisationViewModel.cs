namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Shared;

    public class JoinOrganisationViewModel : YesNoChoiceViewModel
    {
        [Required]
        public Guid OrganisationId { get; set; }

        public string OrganisationName { get; set; }

        [Required(ErrorMessage = "Confirm whether you want to request access to the organisation")]
        public override string SelectedValue { get; set; }
    }
}