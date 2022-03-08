namespace EA.Weee.Requests.Organisations
{
    using System.Collections.Generic;
    using Core.Organisations;
    using Prsd.Core.Mediator;

    public class GetAllOrganisations : IRequest<List<OrganisationNameStatus>>
    {
        public GetAllOrganisations()
        {
        }
    }
}
