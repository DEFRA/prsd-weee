namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using Core.Organisations;
    using Shared;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class AddressPrepopulateViewModel : YesNoChoiceViewModel
    {
        public Guid OrganisationId { get; set; }

        public OrganisationType OrganisationType { get; set; }

        [Required(ErrorMessage = "Tell us if your registered office and main point of contact details are the same")]
        public override string SelectedValue { get; set; }
    }
}