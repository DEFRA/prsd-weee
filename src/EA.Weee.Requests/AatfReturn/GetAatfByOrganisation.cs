namespace EA.Weee.Requests.AatfReturn
{
    using System;

    public class GetAatfByOrganisation
    {
        public Guid OrganisationId { get; set; }

        public GetAatfByOrganisation(Guid organisationId)
        {
            OrganisationId = organisationId;
        }
    }
}
