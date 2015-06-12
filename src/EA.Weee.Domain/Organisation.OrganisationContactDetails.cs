namespace EA.Weee.Domain
{
    using System;

    public partial class Organisation
    {
        public void AddOrganisationContactDetails(Address orgContactDetails)
        {
            OrganisationAddress = orgContactDetails;
        }

        public void RemoveOrganisationContactDetails()
        {
            OrganisationAddress = null;
        }
    }
}
