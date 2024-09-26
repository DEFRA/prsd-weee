namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class OrganisationsFoundViewModel
    {
        public IList<OrganisationFoundViewModel> OrganisationFoundViewModels { get; set; }

        [Required(ErrorMessage = "You must choose an organisation")]
        public Guid? SelectedOrganisationId { get; set; }

        public OrganisationFoundType OrganisationFoundType { get; set; }
    }
}