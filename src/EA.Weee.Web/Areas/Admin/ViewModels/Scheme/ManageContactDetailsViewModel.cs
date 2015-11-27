namespace EA.Weee.Web.Areas.Admin.ViewModels.Scheme
{
    using Core.Organisations;
    using Core.Shared;
    using System;

    public class ManageContactDetailsViewModel
    {   
        public AddressData OrganisationAddress { get; set; }

        public ContactData Contact { get; set; }

        public Guid SchemeId { get; set; }

        public Guid OrgId { get; set; }
    }
}