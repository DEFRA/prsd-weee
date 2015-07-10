namespace EA.Weee.Web.ViewModels.Organisation
{
    using System.Collections.Generic;
    using Core.Organisations;

    public class OrganisationUserPendingViewModel
    {
        public OrganisationUserPendingViewModel()
        {
            OrganisationUserData = new List<OrganisationUserData>();
        }

        public List<OrganisationUserData> OrganisationUserData { get; set; }
    }
}