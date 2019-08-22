namespace EA.Weee.Requests.AatfReturn
{
    using Core.AatfReturn;
    using Prsd.Core.Mediator;
    using System;
    using System.Collections.Generic;

    public class GetAatfByOrganisation : IRequest<List<AatfData>>
    {
        public Guid OrganisationId { get; set; }

        public GetAatfByOrganisation(Guid organisationId)
        {
            OrganisationId = organisationId;
        }
    }
}
