namespace EA.Weee.Web.Areas.Admin.ViewModels.Scheme
{
    using System;
    using Core.Organisations;

    public class ManageContactDetailsViewModel
    {
        public OrganisationData OrganisationData { get; set; }

        public Guid SchemeId { get; set; }
    }
}