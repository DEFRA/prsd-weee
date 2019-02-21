namespace EA.Weee.Requests.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using Core.AatfReturn;
    using Prsd.Core.Mediator;

    public class GetAatfByOrganisation : IRequest<List<AatfData>>
    {
        public Guid OrganisationId { get; set; }

        public GetAatfByOrganisation(Guid organisationId)
        {
            OrganisationId = organisationId;
        }
    }
}
