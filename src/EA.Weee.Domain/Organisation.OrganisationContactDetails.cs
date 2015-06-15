namespace EA.Weee.Domain
{
    using System;
    using EA.Prsd.Core;

    public partial class Organisation
    {
        public void AddOrganisationContactDetails(Address orgContactDetails)
        {
            Guard.ArgumentNotNull(orgContactDetails);
            if (OrganisationAddress != null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        "Cannot add Contact details to Organisation {0}. This organisation already has a Contact details {1}.",
                        this.Id,
                        this.OrganisationAddress.Id));
            }
            OrganisationAddress = orgContactDetails;
        }

        public void RemoveOrganisationContactDetails()
        {
            OrganisationAddress = null;
        }
    }
}
