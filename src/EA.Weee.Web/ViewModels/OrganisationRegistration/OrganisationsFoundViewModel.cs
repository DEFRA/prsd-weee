namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class OrganisationsFoundViewModel
    {
        public IEnumerable<OrganisationFoundViewModel> OrganisationFoundViewModels { get; set; }

        [Required(ErrorMessage = "You must choose an organisation")]
        public Guid? SelectedOrganisationId { get; set; }
    }
}