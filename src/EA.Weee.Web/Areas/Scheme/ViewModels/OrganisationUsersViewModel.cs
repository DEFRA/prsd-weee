namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class OrganisationUsersViewModel
    {
        public IList<KeyValuePair<string, Guid>> OrganisationUsers { get; set; }

        [Required]
        [DisplayName("Select a user to manage")]
        public Guid? SelectedOrganisationUser { get; set; }
    }
}