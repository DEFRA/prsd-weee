namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using System;
    using System.Collections.Generic;
    using Weee.Requests.Organisations;

    public class OrganisationUserPendingViewModel
    {
        public OrganisationUserPendingViewModel()
        {
            OrganisationUserData = new List<OrganisationUserData>();
        }

        public List<OrganisationUserData> OrganisationUserData { get; set; }
    }
}