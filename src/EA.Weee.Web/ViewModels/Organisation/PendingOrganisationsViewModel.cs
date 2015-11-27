namespace EA.Weee.Web.ViewModels.Organisation
{
    using System.Collections.Generic;
    using Core.Organisations;

    public class PendingOrganisationsViewModel
    {
        public IEnumerable<OrganisationUserData> InaccessibleOrganisations { get; set; }
    }
}