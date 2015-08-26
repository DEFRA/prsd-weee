namespace EA.Weee.Web.ViewModels.Organisation
{
    using System.Collections.Generic;
    using Core.Organisations;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Web.ViewModels.Shared;

    public class PendingOrganisationsViewModel
    {
        public IEnumerable<OrganisationUserData> InaccessibleOrganisations { get; set; }
    }
}