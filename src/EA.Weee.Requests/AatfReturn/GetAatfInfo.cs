namespace EA.Weee.Requests.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using Core.AatfReturn;
    using Core.Scheme;
    using Prsd.Core.Mediator;

    public class GetAatfInfoByOrganisation : IRequest<List<AatfData>>
    {
        public Guid OrganisationId { get; set; }
    }
}
