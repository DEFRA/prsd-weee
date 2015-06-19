namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Web.ViewModels.Shared;

    public class AddressPrepopulateViewModel
    {
        [Required]
        public RadioButtonStringCollectionViewModel ContactDetailsSameAs { get; set; }

        public Guid OrganisationId { get; set; }

        public OrganisationType OrganisationType { get; set; }
    }
}