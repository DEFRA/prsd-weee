namespace EA.Weee.Requests.Organisations
{
    using Core.Organisations;
    using Prsd.Core.Mediator;
    using System.Collections.Generic;

    public class GetUserOrganisationsByStatus : IRequest<List<OrganisationUserData>>
    {
        public int[] OrganisationStatus { get; set; }

        public int[] OrganisationUserStatus { get; set; }

        public GetUserOrganisationsByStatus(int[] organisationUserStatus, int[] organisationStatus = null)
        {
            OrganisationUserStatus = organisationUserStatus;
            OrganisationStatus = organisationStatus;
        }
    }
}
