namespace EA.Weee.Requests.Organisations
{
    using System;
    using Core.Organisations;
    using Prsd.Core.Mediator;

    public class OrganisationSearchById : IRequest<OrganisationSearchData>
    {
        public readonly Guid Id;

        public OrganisationSearchById(Guid id)
        {
            Id = id;
        }
    }
}