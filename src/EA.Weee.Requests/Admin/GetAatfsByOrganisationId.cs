namespace EA.Weee.Requests.Admin
{
    using System;
    using System.Collections.Generic;
    using Core.AatfReturn;
    using Prsd.Core.Mediator;

    public class GetAatfsByOrganisationId : IRequest<List<AatfDataList>>
    {
        public Guid OrganisationId { get; set; }

        public GetAatfsByOrganisationId(Guid id)
        {
            OrganisationId = id;
        }
    }
}
