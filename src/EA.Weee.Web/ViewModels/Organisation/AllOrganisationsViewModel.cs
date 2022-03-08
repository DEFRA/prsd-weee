namespace EA.Weee.Web.ViewModels.Organisation
{
    using EA.Weee.Core.Organisations;
    using System;
    using System.Collections.Generic;

    public class AllOrganisationsViewModel
    {
        public IReadOnlyList<OrganisationData> Organisations { get; set; }

        public Guid? SelectedOrganisationId { get; set; }
    }
}