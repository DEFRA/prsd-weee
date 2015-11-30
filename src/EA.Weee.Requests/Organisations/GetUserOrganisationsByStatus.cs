namespace EA.Weee.Requests.Organisations
{
    using System.Collections.Generic;
    using Core.Organisations;
    using Prsd.Core.Mediator;

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
