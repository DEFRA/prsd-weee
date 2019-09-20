namespace EA.Weee.Web.ViewModels.Organisation
{
    using Core.Organisations;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class YourOrganisationsViewModel
    {
        public IReadOnlyList<OrganisationUserData> Organisations { get; set; }

        [DisplayName("Which organisation would you like to perform activities for?")]
        [Required(ErrorMessage = "Select an organisation to perform activities")]
        public Guid? SelectedOrganisationId { get; set; }
    }
}