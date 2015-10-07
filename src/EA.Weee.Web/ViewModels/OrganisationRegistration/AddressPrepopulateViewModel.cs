namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using System;
    using Core.Organisations;
    using Shared;

    public class AddressPrepopulateViewModel : YesNoChoiceViewModel
    {
        public Guid OrganisationId { get; set; }

        public OrganisationType OrganisationType { get; set; }
    }
}