namespace EA.Weee.Requests.Organisations
{
    using System;
    using Prsd.Core.Mediator;
    using System.Collections.Generic;
    using Core.Organisations;

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
