namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Core.Organisations;
    using Shared;

    public class AddressPrepopulateViewModel
    {
        [Required]
        public YesNoChoiceViewModel ContactDetailsSameAs { get; set; }

        public Guid OrganisationId { get; set; }

        public OrganisationType OrganisationType { get; set; }
    }
}