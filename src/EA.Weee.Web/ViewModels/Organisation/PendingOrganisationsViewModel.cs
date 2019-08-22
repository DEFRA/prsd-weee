namespace EA.Weee.Web.ViewModels.Organisation
{
    using Core.Organisations;
    using System.Collections.Generic;

    public class PendingOrganisationsViewModel
    {
        public IEnumerable<OrganisationUserData> InaccessibleOrganisations { get; set; }
    }
}