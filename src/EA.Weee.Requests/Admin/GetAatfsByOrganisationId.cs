namespace EA.Weee.Requests.Admin
{
    using Core.AatfReturn;
    using Prsd.Core.Mediator;
    using System;
    using System.Collections.Generic;

    public class GetAatfsByOrganisationId : IRequest<List<AatfDataList>>
    {
        public Guid OrganisationId { get; set; }

        public GetAatfsByOrganisationId(Guid id)
        {
            OrganisationId = id;
        }
    }
}
