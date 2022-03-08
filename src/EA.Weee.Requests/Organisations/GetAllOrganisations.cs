namespace EA.Weee.Requests.Organisations
{
    using Core.Organisations;
    using Prsd.Core.Mediator;
    using System;
    using System.Collections.Generic;

    public class GetAllOrganisations : IRequest<List<OrganisationData>>
    {
        public GetAllOrganisations()
        {
        }
    }
}
